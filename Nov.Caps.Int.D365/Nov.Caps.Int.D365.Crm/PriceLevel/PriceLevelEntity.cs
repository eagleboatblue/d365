using System;
using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;

namespace Nov.Caps.Int.D365.Crm.PriceLevel
{
    public class PriceLevelEntity : Entity
    {
        [JsonProperty("pricelevelid", Required = Required.Default)]
        public Guid? ID { get; set; }
        
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }
    }
}
