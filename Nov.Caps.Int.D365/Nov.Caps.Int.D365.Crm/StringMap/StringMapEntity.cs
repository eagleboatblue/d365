using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
using Nov.Caps.Int.D365.Crm.Core.OData;
using System;
namespace Nov.Caps.Int.D365.Crm.StringMap
{
    public class StringMapEntity : Entity
    {        
        [JsonProperty("attributename", Required = Required.Always)]
        public string AttName { get; set; }

    	[JsonProperty("attributevalue", Required = Required.Always)]
        public int AttValue { get; set; }

        [JsonProperty("value", Required = Required.Always)]
        public string Value { get; set; }
        
    	[JsonProperty("objecttypecode", Required = Required.Always)]
        public string EntityName { get; set; }
    }
}
