using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
using Nov.Caps.Int.D365.Crm.Core.OData;
using System;
namespace Nov.Caps.Int.D365.Crm.DefaultUnit
{
    public class DefaultUnitEntity : Entity
    {
        [JsonProperty("uomid", Required = Required.Default)]
        public Guid? ID { get; set; }

        [JsonProperty("_uomscheduleid_value", Required = Required.Default)]
        [ReferenceProperty("uomscheduleid")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "uomschedules" })]
        public Guid? UnitGroup { get; set; }		

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }
        
		[JsonProperty("quantity", Required = Required.Always)]
        public decimal Quantity { get; set; }                
    }
}
