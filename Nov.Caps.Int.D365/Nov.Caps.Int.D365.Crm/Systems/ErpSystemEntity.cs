using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
using Nov.Caps.Int.D365.Crm.Core.OData;
using System;
namespace Nov.Caps.Int.D365.Crm.Systems
{
    public class ErpSystemEntity : Entity
    {
        [JsonProperty("ava_erpsystemid", Required = Required.Default)]
        public Guid? ID { get; set; }

        [JsonProperty("ava_activeerp", Required = Required.Default)]
        public string ActiveERP { get; set; }

        [JsonProperty("ava_businessunitcode", Required = Required.Default)]
        public Guid? BusinessUnitCodeID { get; set; }

        [JsonProperty("ava_businessunitdescription", Required = Required.Default)]
        public string BusinessUnitDescription { get; set; }

        [JsonProperty("ava_businesswk", Required = Required.Default)]
        public string BusinessWK { get; set; }

        [JsonProperty("ava_default", Required = Required.Default)]
        public string DefaultLedgerDescription { get; set; }

        [JsonProperty("ava_defaultbusinessunitid", Required = Required.Default)]
        public string DefaultBusinessUnitID { get; set; }

        [JsonProperty("ava_defaultledgernumber", Required = Required.Default)]
        public string DefaultLedgerNumber { get; set; }

        [JsonProperty("ava_erpcode", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("ava_erpcustomerprefix", Required = Required.Default)]
        public string CustomerPrefix { get; set; }

        [JsonProperty("ava_erpsystemwk", Required = Required.Default)]
        public string WK { get; set; }

        [JsonProperty("_ava_parentsystem_value", Required = Required.Default)]
        [ReferenceProperty("ava_parentsystem")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "ava_erpparentsystem" })]
        public Guid? ParentID { get; set; }

        [JsonProperty("ava_servername", Required = Required.Default)]
        public string ServerName { get; set; }

        [JsonProperty("ava_systemdesc", Required = Required.Default)]
        public string Description { get; set; }
    }
}
