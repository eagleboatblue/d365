using Dapper;
using Nov.Caps.Int.D365.Common;
using Nov.Caps.Int.D365.Data.Abstract;
using Nov.Caps.Int.D365.Data.Common;
using Nov.Caps.Int.D365.Data.Queries;
using Nov.Caps.Int.D365.Models.Common;
using Nov.Caps.Int.D365.Models.Products;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;


namespace Nov.Caps.Int.D365.Data.Incorta
{
    public class ProductRepository : IProductRepository
    {
        private readonly ConnectionSettings settings;

        public ProductRepository(ConnectionSettings settings)
        {
            this.settings = settings;
        }

        public ChannelReader<Result<Product>> FindAsync(CancellationToken cancellation)
        {
            var ch = Channel.CreateBounded<Result<Product>>(1);

            Task.Run(async () => {
                try
                {
                    
                    using (var conn = new NpgsqlConnection(this.settings.ToString()))
                    {
                        await conn.OpenAsync();

                        var sb = new StringBuilder();
                        sb.AppendLine(@"
                        SELECT 
                            a.BUSINESS_UNIT as BusinessUnit,
                            a.PRODUCT_GROUP as ProductGroup,
                            a.PRODUCT_LINE as ProductLine,
                            a.MINOR_CATEGORY_1 as MinorCategory1,
                            a.MINOR_CATEGORY_2 as MinorCategory2,
                            a.PRODUCT_ID as PartNumber,
                            a.MANUFACTURING_PART_NUMBER as MfgId,
                            a.ERP_ID as ErpId,                            
                            a.ACTIVE_SELLABLE as ActiveSellable,
							CASE
                                    WHEN a.ACTIVE_SELLABLE = 'Yes' THEN 0
                                    WHEN a.ACTIVE_SELLABLE = 'No' THEN 3                                    
                            END State,
                            CASE
                                    WHEN a.ACTIVE_SELLABLE = 'Yes' THEN 1
                                    WHEN a.ACTIVE_SELLABLE = 'No' THEN 3                                    
                            END Status,                         
                            a.DESCRIPTION as ShortDesc,
                            a.SALES_DESCRIPTION as SalesDesc,
                            a.WAREHOUSE_CODE as WhCode,
                            a.PRODUCT_TYPE as CatName,
                            a.DEFAULT_UNIT as DefaultUnit,
                            a.PROD_GRP_A as GrpA,
                            a.PROD_GRP_B as GrpB,
                            a.PROD_GRP_C as GrpC 
                            FROM UDS_CAPS_CX.MV_D365_COMBINED_DATA a
                            WHERE (date(a.LAST_MODIFIED_DATE) > current_date - interval '2 day')"
                    );
                        
                       
                        var result = conn.Query<Product>(sb.ToString(), buffered: true, commandTimeout: 0);

                        foreach (var product in result)
                        {
                            if (cancellation.IsCancellationRequested)
                            {
                                break;
                            }

                            await ch.Writer.WaitToWriteAsync();
                            await ch.Writer.WriteAsync(new Result<Product>(product));
                        }
                    }
                } catch (Exception ex)
                {
                    await ch.Writer.WaitToWriteAsync();
                    await ch.Writer.WriteAsync(new Result<Product>(ex));
                } finally
                {
                    ch.Writer.Complete();
                }
				
               
            }, cancellation);

            return ch.Reader;
        }
    }
}
