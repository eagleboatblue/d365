using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
using System;

namespace Nov.Caps.Int.D365.Crm.Currencies
{
    public class TransactionCurrencyEntity : Entity
    {
        [JsonProperty("transactioncurrencyid", Required = Required.Default)]
        public Guid? ID { get; set; }

        [JsonProperty("isocurrencycode", Required = Required.Always)]
        public string Code { get; set; }

        [JsonProperty("currencyname", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("exchangerate", Required = Required.Always)]
        public double Rate { get; set; }

        [JsonProperty("currencyprecision", Required = Required.Always)]
        public int Precision { get; set; }

        [JsonProperty("currencysymbol", Required = Required.Always)]
        public string Symbol { get; set; }
    }
}
