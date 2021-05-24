using System;
using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
namespace Nov.Caps.Int.D365.Crm.Systems
{
    public class ErpParentSystemEntity : Entity
    {
        [JsonProperty("ava_erpparentsystemid", Required = Required.Default)]
        public Guid? ID { get; set; }

        [JsonProperty("ava_name", Required = Required.Always)]
        public string Name { get; set; }
    }
}
