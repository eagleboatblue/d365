using System;
using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;

namespace Nov.Caps.Int.D365.Crm.Businesses
{
    public class BusinessUnitEntity: Entity
    {
        [JsonProperty("businessunitid", Required = Required.Default)]
        public Guid? ID { get; set; }

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }
    }
}
