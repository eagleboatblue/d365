using Serilog;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Nov.Caps.Int.D365.Data.Abstract;
using Nov.Caps.Int.D365.Models.Products;
using Nov.Caps.Int.D365.Crm.Products;
using Nov.Caps.Int.D365.Crm.Businesses;
using Nov.Caps.Int.D365.Crm.ProductGroup;
using Nov.Caps.Int.D365.Crm.ProductLine;
using Nov.Caps.Int.D365.Crm.MinorCategory1;
using Nov.Caps.Int.D365.Crm.MinorCategory2;
using Nov.Caps.Int.D365.Crm.UnitGroup;
using Nov.Caps.Int.D365.Crm.DefaultUnit;
using Nov.Caps.Int.D365.Crm.StringMap;
using Nov.Caps.Int.D365.Crm.PriceLevel;
using Nov.Caps.Int.D365.Crm.PriceListItems;

namespace Nov.Caps.Int.D365.Services.Products
{
    public class ProductSyncService
    {
        private readonly ILogger logger;
        private readonly IProductRepository productRepository;        
        private readonly ErpProductService productsService;
        private readonly BusinessUnitService businessUnitService;
        private readonly ProductGroupService productGroupService;
        private readonly ProductLineService productLineService;
		private readonly MinorCategory1Service minorCategory1Service;
		private readonly MinorCategory2Service minorCategory2Service;
        private readonly UnitGroupService unitGroupService;
        private readonly DefaultUnitService defaultUnitService;
        private readonly StringMapService stringMapService;
        private readonly PriceLevelService priceListService;
        private readonly PriceListItemsService priceListItemsService;
 
        enum ProductSyncResult
            {
                Create,
                Update,
                Skip
            };       

        public ProductSyncService(
            ILogger logger,
            IProductRepository productRepository,
            ErpProductService productsService,
            BusinessUnitService businessUnitService,
            ProductGroupService productGroupService,
            ProductLineService productLineService,
            MinorCategory1Service minorCategory1Service,
            MinorCategory2Service minorCategory2Service,
            UnitGroupService unitGroupService,
            DefaultUnitService defaultUnitService,
            StringMapService stringMapService,
            PriceLevelService priceListService,
            PriceListItemsService priceListItemsService
        )
        {
            this.logger = logger.ForContext("service", "products_sync");
            this.productRepository = productRepository;
            this.productsService = productsService;
            this.businessUnitService = businessUnitService;
            this.productGroupService = productGroupService;
            this.productLineService = productLineService;
            this.minorCategory1Service = minorCategory1Service;
            this.minorCategory2Service = minorCategory2Service;
            this.unitGroupService = unitGroupService;
            this.defaultUnitService = defaultUnitService;
            this.stringMapService = stringMapService;
            this.priceListService = priceListService;
            this.priceListItemsService = priceListItemsService;            
        }

        public async Task Sync()
        {
            var cancellation = new CancellationTokenSource();
            var failedCodes = new List<string>();
            var totalCounter = 0;
            var createdCounter = 0;
            var updatedCounter = 0;
            var skippedCounter = 0;

            try
            {
                var ch = this.productRepository.FindAsync(cancellation.Token);

                while (await ch.WaitToReadAsync())
                {
                    var result = await ch.ReadAsync();

                    if (!result.IsOk)
                    {
                        throw result.Exception;
                    }

                    Product product = result.Data;

                    try
                    {
                        totalCounter++;
                        var created = await this.resolveProduct(product);                        

                        if (created == ProductSyncResult.Create)
                        {
                            createdCounter++;

                            this.logger.Information("Successfully created a new product {PartNumber}", product.PartNumber);
                        }
                        else if (created == ProductSyncResult.Update)
                        {
                            updatedCounter++;
                            this.logger.Information("Successfully updated a product {PartNumber}", product.PartNumber);
                        }
                        else
                        {
                            skippedCounter++;
                            this.logger.Information("Product {PartNumber} "+ product.PartNumber + "not updated");
                        }
                    }
                    catch (Exception ex)
                    {
                        failedCodes.Add(product.PartNumber);
                        this.logger.Error("Failed to sync product {@product} {@error}", product, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Error("Failed to sync product {@error} {total}", ex, totalCounter);

                throw ex;
            }
            finally
            {
                cancellation.Cancel();
            }

            if (failedCodes.Count == 0)
            {
                if ((createdCounter + updatedCounter + skippedCounter) > 0)
                {
                    this.logger.Information("Successfully synced products {created} {updated} {noupdate} {total}", createdCounter, updatedCounter, skippedCounter, totalCounter);
                }
                else
                {
                    this.logger.Information("Products are in sync {total}", totalCounter);
                }

                return;
            }

            if (failedCodes.Count <= 10)
            {
                var codes = string.Join(", ", failedCodes);

                this.logger.Information("Failed to sync the following products {PartNumber} {created} {updated} {notupdated} {total}", codes, createdCounter, updatedCounter, skippedCounter, totalCounter);

                throw new Exception($"Failed to sync the following products: {codes}");
            }

            this.logger.Information("Failed to sync products {failed} {created} {updated} {notupdated} {total}", failedCodes.Count, createdCounter, updatedCounter, skippedCounter, totalCounter);

            throw new Exception($"Failed to sync {failedCodes.Count} of {totalCounter} product(s)");
        }

        private async Task<ProductSyncResult> resolveProduct(Product product)
        {
            
            var found = await this.productsService.FindBy(new ErpProductFilter()
            {
                ProductNumber = product.PartNumber,                
            });

            // if a customer account already exists
            if (found.Any())
            {
                var updatedProduct = await this.createOrUpdateProduct(found.First(), product, ProductSyncResult.Update);  
                await this.createPriceListItems(updatedProduct);              
                return (ProductSyncResult.Update);                
            }
            else if (product.State==0)
			{                
                var updatedProduct = await this.createOrUpdateProduct(new ErpProductEntity(), product, ProductSyncResult.Create);
                await this.createPriceListItems(updatedProduct);

                //Updating to set the Default Price List
                await this.productsService.UpdateAsync(updatedProduct);

                return (ProductSyncResult.Create);
            }
            else
                return (ProductSyncResult.Skip);
        }

        private async Task<ErpProductEntity> createOrUpdateProduct(ErpProductEntity product, Product productData, ProductSyncResult code) {
               
            var ProdType = new StringMapEntity();

            //Resolve Business Unit
            var businessUnit = await this.resolveBusinessUnit(productData);

            //Replacing '&' character suitable for query
            if (!String.IsNullOrWhiteSpace(productData.ProductGroup))
            {
                productData.ProductGroup = resolveSpecialCharacters(productData.ProductGroup);
            }

            if (businessUnit != null)
            {
                product.BusinessUnit = productData.BusinessUnitID = businessUnit.ID;
            }

            //Resolve Product group
            var productGroup = await this.resolveProductGroup(productData);

            if (productGroup != null)
            {
                product.ProductGroup = productData.ProductGroupID = productGroup.ID;
            }
 
            //Check and create ProductLine
            var productLine = await this.resolveProductLine(productData);

            if (productLine != null)
            {
                product.ProductLine = productData.ProductLineID = productLine.ID;
            }

            //Check and create MinorCategory1
            var minorCategory1 = await this.resolveMinorCategory1(productData);

            if (minorCategory1 != null)
            {
                product.MinorCategory1 = productData.MinorCategory1ID = minorCategory1.ID;
            }

            //Check and create MinorCategory2
            var minorCategory2 = await this.resolveMinorCategory2(productData);

            if (minorCategory2 != null)
            {
                product.MinorCategory2 = minorCategory2.ID;
            }

            //Resolve Unit Group
            productData.UnitGroup = "Default Unit";
            var unitGroup = await this.resolveUnitGroup(productData);

            if (unitGroup != null)
            {
                product.DefaultUoMScheduleId = productData.UnitGroupID = unitGroup.ID;
            }


            //Check and create DefaultUnit
            var defaultUnit = await this.resolveDefaultUnit(productData);

            if (defaultUnit != null)
            {
                product.DefaultUnit = defaultUnit.ID;
            }

            //Resolve Default Price List
            productData.DefaultPriceList = "CAPS";
            var defaultPriceList = await this.resolveDefaultPriceList(productData.DefaultPriceList);

            if (defaultPriceList != null)
            {
                product.PriceLevelId = defaultPriceList.ID;
            }

            //Resolve Product Type            
            if (productData.CatName != null)
            {
                ProdType = await this.resolveStringMap(productData.CatName, "product", "ava_producttype");
            }
            else
            {
                ProdType.AttValue = 181910000;
            }

            //Resolve Warehouse Code
            if (productData.WhCode != null)
            {
                var WhCode = await this.resolveStringMap(productData.WhCode, "product", "ava_warehousecode");
                if (WhCode != null)
                {
                    product.WarehouseCode = WhCode.AttValue;
                if (product.WarehouseCode == 0)
                {
                    product.WarehouseCode = null;
                }
                }
                else 
                {
                    product.WarehouseCode = null;
                }
            }
            else
            {
                product.WarehouseCode = null;
            }

            //Replacing Special Characters suitable for query
            if (!String.IsNullOrWhiteSpace(productData.SalesDesc))
            {
                productData.SalesDesc = resolveSpecialCharacters(productData.SalesDesc);

            }

            //Replacing Special Characters suitable for query
            if (!String.IsNullOrWhiteSpace(productData.ShortDesc))
            {
                productData.ShortDesc = resolveSpecialCharacters(productData.ShortDesc);
            }           

            product.ProductNumber = productData.PartNumber;
            product.ManufacturingPartNumber = productData.MfgId;
            product.ErpId = productData.ErpId;
            product.StatusReason = productData.State;
            product.Status = productData.Status;
            product.Name = productData.ShortDesc;
            product.Description = productData.SalesDesc;            
            product.ProductType = ProdType.AttValue;
            product.ProdGrpA = productData.GrpA;
            product.ProdGrpB = productData.GrpB;
            product.ProdGrpC = productData.GrpC;
            product.Quantity = productData.Quantity = 2;
            product.writein = productData.Writein = false;

            if (code.Equals(ProductSyncResult.Update))
            {
                await this.productsService.UpdateAsync(product);
            }
            else
            {
                await this.productsService.CreateAsync(product);
            }
            return product;
        }
        

        private async Task<BusinessUnitEntity> resolveBusinessUnit(Product product) {
            if (!String.IsNullOrWhiteSpace(product.BusinessUnit))
            {
                var foundBusinessUnits = await this.businessUnitService.FindByNameAsync(product.BusinessUnit);

                if (foundBusinessUnits.Any())
                {
                    return foundBusinessUnits.First();
                }
                else
                {
                    this.logger.Warning("Unable to resolve business unit {business_unit}", product.BusinessUnit);
                    return null;
                }                
            }
            return null;
        }
        
        private async Task<ProductGroupEntity> resolveProductGroup(Product product) {
            if (!String.IsNullOrWhiteSpace(product.ProductGroup))
            {
                var foundProductGroup = await this.productGroupService.FindByNameAsync(product.ProductGroup,product.BusinessUnitID);

                if (foundProductGroup.Any())
                {
                    return foundProductGroup.First();
                }
                else
                {
                    this.logger.Warning("Unable to resolve ProductGroup {product_group}", product.ProductGroup);
                    return null;
                }
            }
            return null;
        }
        
        private async Task<ProductLineEntity> resolveProductLine(Product product) {
            if (!String.IsNullOrWhiteSpace(product.ProductLine))
            {
                var foundProductLine = await this.productLineService.FindByProductLineAsync(product.ProductLine, product.BusinessUnitID, product.ProductGroupID);

                if (foundProductLine.Any())
                {
                    return foundProductLine.First();
                }
                this.logger.Warning("Unable to resolve ProductLine {product_line}", product.ProductLine);                    
                  
            
                this.logger.Information("Creating Product Line: {id}", product.ProductLine);
			    
                var productLine = new ProductLineEntity();
                productLine.BusinessUnit = product.BusinessUnitID;
                productLine.ProductGroup = product.ProductGroupID;
                productLine.ProductLine = product.ProductLine;
                
                await this.productLineService.CreateAsync(productLine);

                 //Retrieve ID
                var productLineID = await this.minorCategory1Service.FindByMinorCategory1Async(product.MinorCategory1, product.BusinessUnitID, product.ProductGroupID, product.ProductLineID);

                if (productLineID.Any())
                {
                   productLine.ID = productLineID.First().ID;
                   return productLine;            
                }
            }
                return null;               
        }
        
		
		private async Task<MinorCategory1Entity> resolveMinorCategory1(Product product) {

            if (!String.IsNullOrWhiteSpace(product.MinorCategory1))
            {
                var foundMinorCategory1 = await this.minorCategory1Service.FindByMinorCategory1Async(product.MinorCategory1, product.BusinessUnitID, product.ProductGroupID, product.ProductLineID);

                if (foundMinorCategory1.Any())
                {
                    return foundMinorCategory1.First();
                }
                this.logger.Warning("Unable to resolve MinorCategory1 {minorcategory1}", product.MinorCategory1);
                this.logger.Information("Creating MinorCategory1: {id}", product.MinorCategory1);
                
                var minorCategory1 = new MinorCategory1Entity();
                minorCategory1.BusinessUnit = product.BusinessUnitID;
                minorCategory1.ProductGroup = product.ProductGroupID;
                minorCategory1.ProductLine = product.ProductLineID;
                minorCategory1.MinorCategory1 = product.MinorCategory1;
                
                await this.minorCategory1Service.CreateAsync(minorCategory1); ;

                 //Retrieve ID
                var minorCategory1ID = await this.minorCategory1Service.FindByMinorCategory1Async(product.MinorCategory1, product.BusinessUnitID, product.ProductGroupID, product.ProductLineID);

                if (minorCategory1ID.Any())
                {
                   minorCategory1.ID = minorCategory1ID.First().ID;
                   return minorCategory1;            
                }
            }
                return null;
               
        }
		
		private async Task<MinorCategory2Entity> resolveMinorCategory2(Product product) {
            if (!String.IsNullOrWhiteSpace(product.MinorCategory2))
            {
                var foundMinorCategory2 = await this.minorCategory2Service.FindByMinorCategory2Async(product.MinorCategory2,product.MinorCategory1ID, product.BusinessUnitID, product.ProductGroupID, product.ProductLineID);

                if (foundMinorCategory2.Any())
                {
                    return foundMinorCategory2.First();
                }
                    this.logger.Warning("Unable to resolve MinorCategory2 {minorcategory2}", product.MinorCategory2);
                    this.logger.Information("Creating MinorCategory2: {id}", product.MinorCategory2);
                    
                    var minorCategory2 = new MinorCategory2Entity();
                    minorCategory2.BusinessUnit = product.BusinessUnitID;
                    minorCategory2.ProductGroup = product.ProductGroupID;
                    minorCategory2.ProductLine = product.ProductLineID;
			        minorCategory2.MinorCategory1 = product.MinorCategory1ID;	
                    minorCategory2.MinorCategory2 = product.MinorCategory2;
                    
                await this.minorCategory2Service.CreateAsync(minorCategory2); 

                //Retrieve ID
                var minorCategory2ID = await this.minorCategory2Service.FindByMinorCategory2Async(product.MinorCategory2,product.MinorCategory1ID, product.BusinessUnitID, product.ProductGroupID, product.ProductLineID);

                if (minorCategory2ID.Any())
                {
                   minorCategory2.ID = minorCategory2ID.First().ID;
                   return minorCategory2;            
                }
            }
                return null;               
        }

        private async Task<UnitGroupEntity> resolveUnitGroup(Product product)
        {
            if (!String.IsNullOrWhiteSpace(product.UnitGroup))
            {
                var foundUnitGroup = await this.unitGroupService.FindByNameAsync(product.UnitGroup);

                if (foundUnitGroup.Any())
                {
                    return foundUnitGroup.First();
                }
                else
                {
                    this.logger.Warning("Unable to resolve Unit Group {unit_group}", product.UnitGroup);
                    return null;
                }
            }
            return null;
        }


        private async Task<DefaultUnitEntity> resolveDefaultUnit(Product product) {
            if (!String.IsNullOrWhiteSpace(product.DefaultUnit))
            {
                var foundDefaultUnit = await this.defaultUnitService.FindByDefaultUnitAsync(product.DefaultUnit, product.UnitGroupID);

                if (foundDefaultUnit.Any())
                {
                    return foundDefaultUnit.First();
                }
                    this.logger.Warning("Unable to resolve foundDefaultUnit {default_unit}", product.DefaultUnit);
                    this.logger.Information("Creating DefaultUnit: {id}", product.DefaultUnit);
                    
                    var defaultUnit = new DefaultUnitEntity();
                    defaultUnit.UnitGroup = product.UnitGroupID;
                    defaultUnit.Name = product.DefaultUnit;
                    defaultUnit.Quantity = product.Quantity;
                    
                    await this.defaultUnitService.CreateAsync(defaultUnit);

                //Retrieve ID
                var defaultUnitID = await this.defaultUnitService.FindByDefaultUnitAsync(product.DefaultUnit, product.UnitGroupID);

                if (defaultUnitID.Any())
                {
                   defaultUnit.ID = defaultUnitID.First().ID;
                   return defaultUnit;            
                }
            }
                return null;
               
        }

        private async Task<PriceLevelEntity> resolveDefaultPriceList(string priceList)
        {
            if (!String.IsNullOrWhiteSpace(priceList))
            {
                var foundPriceList = await this.priceListService.FindByNameAsync(priceList);

                if (foundPriceList.Any())
                {
                    return foundPriceList.First();
                }
                else
                {
                    this.logger.Warning("Unable to resolve Default Price List {price_list}", priceList);
                    return null;
                }
            }
            return null;
        }

        private async Task<StringMapEntity> resolveStringMap(String id, String entity, string attribute) {
			try {
            var foundStringMap = await this.stringMapService.FindByValueAsync(id,entity, attribute);

            if (foundStringMap.Any())
            {
                return foundStringMap.First();
            }
                return null;
			}
           catch (Exception ex)
           {
           this.logger.Error("Could not find StringMap by id: {id}, {@error}", id, ex);
           }
            return null;
        }

        private string resolveSpecialCharacters(String value) 
        {
		   try 
           {
                value = Regex.Replace(value,"&", "%26");  
                value = Regex.Replace(value,"/", "%2F");
                value = Regex.Replace(value, "\\+", "%2F");
                value = Regex.Replace(value,"\\?", "%3F");               
                value = Regex.Replace(value,"\\#", "%23"); 
		   }
           catch (Exception ex)
           {
                this.logger.Error("Error occurred in resolving Special Characters {error}", ex );
           }
            return value;
        }

        private async Task createPriceListItems(ErpProductEntity product)
        {
           this.logger.Information("Creating Price list items for Product : {product.PartNumber}");
           string[] priceList = { "CAPS", "CAPS (AUD)", "CAPS (AED)", "CAPS (SGD)", "CAPS (SAR)", "CAPS (GBP)", "CAPS (NOK)" };
           decimal amount = 0.00M;
           for (int i = 0; i < priceList.Length; i++)
           {
                //Resolve Price List  
                var priceListRef = await this.priceListService.FindByNameAsync(priceList[i]);
                var priceListProductRef = await this.priceListItemsService.FindByNameAsync(priceListRef.First().ID, product.ID);

                if (priceListProductRef.Any())
                {
                    continue;
                }
                else
                {
                    var PriceListItem = new PriceListItemsEntity();
                    PriceListItem.PriceLevel = priceListRef.First().ID;
                    PriceListItem.ProductID = product.ID;               
                    PriceListItem.Unit = product.DefaultUnit;
                    PriceListItem.QuantityCode = 1;
                    PriceListItem.PricingMethod = 1;
                    PriceListItem.Amount = amount;

                try 
                {
                    await this.priceListItemsService.CreateAsync(PriceListItem);
                }
                catch (Exception err)
                {
                    System.Console.WriteLine("error is" + err.StackTrace + err.Message + err.InnerException);
                }
                }                
           }
        }
    }
    }
