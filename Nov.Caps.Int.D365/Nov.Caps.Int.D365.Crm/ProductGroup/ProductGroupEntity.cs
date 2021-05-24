using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
using Nov.Caps.Int.D365.Crm.Core.OData;
using System;

namespace Nov.Caps.Int.D365.Crm.ProductGroup
{
    public class ProductGroupEntity : Entity
    {
        [JsonProperty("ava_productgroupid", Required = Required.Default)]
        public Guid? ID { get; set; }

        [JsonProperty("ava_name", Required = Required.Always)]
        public string Name { get; set; }
        
        [JsonProperty("_ava_businessunitid_value", Required = Required.Default)]
        [ReferenceProperty("ava_BusinessUnitId")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "businessunits" })]
        public Guid? BusinessUnit { get; set; }     
    }
}
