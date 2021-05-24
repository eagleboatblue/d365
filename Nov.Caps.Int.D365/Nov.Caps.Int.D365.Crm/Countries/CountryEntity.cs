using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
using System;
namespace Nov.Caps.Int.D365.Crm.Countries
{
    public class CountryEntity : Entity
    {
        [JsonProperty("nov_countryid", Required = Required.Default)]
        public Guid ID { get; private set; }

        [JsonProperty("sparkh_countrycode", Required = Required.Default)]
        public int Code { get; private set; }

        [JsonProperty("nov_name", Required = Required.Default)]
        public string Name { get; private set; }

        [JsonProperty("importsequencenumber", Required = Required.Default)]
        public int ImportSequenceNumber { get; private set; }
    }
}
