using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
using Nov.Caps.Int.D365.Crm.Core.OData;
using System;
namespace Nov.Caps.Int.D365.Crm.ProductLine
{
    public class ProductLineEntity : Entity
    {        
        [JsonProperty("ava_productlineid", Required = Required.Default)]
        public Guid? ID { get; set; }

        [JsonProperty("_ava_businessunit_value", Required = Required.Default)]
        [ReferenceProperty("ava_BusinessUnit")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "businessunits" })]
        public Guid? BusinessUnit { get; set; }

        [JsonProperty("_ava_productgroupid_value", Required = Required.Default)]
        [ReferenceProperty("ava_ProductGroupId")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "ava_productgroups" })]
        public Guid? ProductGroup { get; set; }

        [JsonProperty("ava_name", Required = Required.Always)]
        public string ProductLine { get; set; }

    }
}
