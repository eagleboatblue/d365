using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
using Nov.Caps.Int.D365.Crm.Core.OData;
using System;
using System.Collections.Generic;

namespace Nov.Caps.Int.D365.Crm.Products
{
    public class ErpProductEntity : Entity
    {
        [JsonProperty("productid", Required = Required.Default)]
        public Guid? ID { get; set; }

        [JsonProperty("_ava_businessunit_value", Required = Required.Default)]
        [ReferenceProperty("ava_BusinessUnit")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "businessunits" })]
        public Guid? BusinessUnit { get; set; }

        [JsonProperty("_ava_productgroupid_value", Required = Required.Default)]
        [ReferenceProperty("ava_ProductGroupId")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "ava_productgroups" })]
        public Guid? ProductGroup { get; set; }

        [JsonProperty("_ava_productline_value", Required = Required.Default)]
        [ReferenceProperty("ava_ProductLine")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "ava_productlines" })]
        public Guid? ProductLine { get; set; }

        [JsonProperty("_ava_minorcategory1_value", Required = Required.Default)]
        [ReferenceProperty("ava_MinorCategory1")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "ava_minorcategory1s" })]
        public Guid? MinorCategory1 { get; set; }

        [JsonProperty("_ava_minorcategory2_value", Required = Required.Default)]
        [ReferenceProperty("ava_MinorCategory2")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "ava_minorcategory2s" })]
        public Guid? MinorCategory2 { get; set; }    

        [JsonProperty("productnumber", Required = Required.Always)]
        public string ProductNumber { get; set; }   

        [JsonProperty("ava_manufacturingpartnumber", Required = Required.Default)]
        public string ManufacturingPartNumber { get; set; }  

        [JsonProperty("ava_erpid", Required = Required.Default)]
        public string ErpId { get; set; }        

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }       

        [JsonProperty("description", Required = Required.Default)]
        public string Description { get; set; }     

        [JsonProperty("ava_warehousecode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? WarehouseCode { get; set; }

        [JsonProperty("ava_producttype", Required = Required.Default)]
        public int ProductType { get; set; }		

        [JsonProperty("_defaultuomid_value", Required = Required.Always)]
        [ReferenceProperty("defaultuomid")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "uoms" })]
        public Guid? DefaultUnit { get; set; }  

        [JsonProperty("ava_prodgrpa", Required = Required.Default)]
        public string ProdGrpA { get; set; }     

        [JsonProperty("ava_prodgrpb", Required = Required.Default)]
        public string ProdGrpB { get; set; }    

        [JsonProperty("ava_prodgrpc", Required = Required.Default)]
        public string ProdGrpC { get; set; }        

        [JsonProperty("_defaultuomscheduleid_value", Required = Required.Always)]
        [ReferenceProperty("defaultuomscheduleid")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "uomschedules" })]
        public Guid? DefaultUoMScheduleId { get; set; }     

        [JsonProperty("_pricelevelid_value", Required = Required.Default)]
        [ReferenceProperty("pricelevelid")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "pricelevels" })]
        public Guid? PriceLevelId { get; set; }       

        [JsonProperty("quantitydecimal", Required = Required.Always)]
        public int Quantity { get; set; }       
         
        [JsonProperty("ava_iswritein", Required = Required.Default)]
        public bool writein { get; set; }    
	}
}
