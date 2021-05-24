using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
using Nov.Caps.Int.D365.Crm.Core.OData;
using System;
namespace Nov.Caps.Int.D365.Crm.MinorCategory2
{
    public class MinorCategory2Entity : Entity
    {
        [JsonProperty("ava_minorcategory2id", Required = Required.Default)]
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

        [JsonProperty("ava_name", Required = Required.Always)]
        public string MinorCategory2 { get; set; }

    }
}
