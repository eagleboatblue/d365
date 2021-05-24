using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
using Nov.Caps.Int.D365.Crm.Core.OData;
using System;
namespace Nov.Caps.Int.D365.Crm.MinorCategory1
{
    public class MinorCategory1Entity : Entity
    {
        [JsonProperty("ava_minorcategory1id", Required = Required.Default)]
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

        [JsonProperty("ava_name", Required = Required.Always)]
        public string MinorCategory1 { get; set; }

    }
}
