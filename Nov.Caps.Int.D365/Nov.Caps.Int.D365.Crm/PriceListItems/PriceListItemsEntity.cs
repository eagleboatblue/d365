using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
using Nov.Caps.Int.D365.Crm.Core.OData;
using System;

namespace Nov.Caps.Int.D365.Crm.PriceListItems
{
    public class PriceListItemsEntity
    {
        [JsonProperty("productpricelevelid", Required = Required.Default)]
        public Guid? ID { get; set; }

        [JsonProperty("_uomid_value", Required = Required.Always)]
        [ReferenceProperty("uomid")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "uoms" })]
        public Guid? Unit { get; set; }

        [JsonProperty("_productid_value", Required = Required.Always)]
        [ReferenceProperty("productid")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "products" })]
        public Guid? ProductID { get; set; }

        [JsonProperty("_pricelevelid_value", Required = Required.Always)]
        [ReferenceProperty("pricelevelid")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "pricelevels" })]
        public Guid? PriceLevel { get; set; }

        [JsonProperty("quantitysellingcode", Required = Required.Always)]
        public int QuantityCode { get; set; } 

     	[JsonProperty("amount", Required = Required.Always)]
        public decimal Amount { get; set; }
        
        [JsonProperty("pricingmethodcode", Required = Required.Always)]
        public int PricingMethod { get; set; } 
    }
}
