using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Common;
using Nov.Caps.Int.D365.Crm.Core.OData;
using System;
using System.Collections.Generic;

namespace Nov.Caps.Int.D365.Crm.Accounts
{
    public class ErpAccountEntity : Entity
    {
        [JsonProperty("ava_erpaccountid", Required = Required.Default)]
        public Guid? ID { get; set; }

        [JsonProperty("ava_capsid", Required = Required.Default)]
        public int? CapsID { get; set; }

        [JsonProperty("ava_city", Required = Required.Default)]
        public string City { get; set; }

        [JsonProperty("ava_creditlimit", Required = Required.Default)]
        public decimal CreditLimit { get; set; }

        [JsonProperty("ava_creditlimit_text", Required = Required.Default)]
        public string CreditLimitText { get; set; }

        [JsonProperty("_ava_currencyid_value", Required = Required.Default)]
        [ReferenceProperty("ava_CurrencyId")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "transactioncurrencies" })]
        public Guid? CurrencyID { get; set; }

        [JsonProperty("ava_customercode", Required = Required.Default)]
        public string CustomerCode { get; set; }

        [JsonProperty("ava_customerid", Required = Required.Default)]
        public string CustomerName { get; set; }

        [JsonProperty("ava_defaultpaymentcode", Required = Required.Default)]
        public string DefaultPaymentCode { get; set; }

        [JsonProperty("ava_dwcustomerid", Required = Required.Default)]
        public int DWCustomerID { get; set; }

        [JsonProperty("ava_dwcustomerid_text", Required = Required.Default)]
        public string DWCustomerIDText { get; set; }

        [JsonProperty("ava_dwsource", Required = Required.Default)]
        public int DWSource { get; set; }

        [JsonProperty("_ava_erpsystemcode_value", Required = Required.Default)]
        [ReferenceProperty("ava_ERPSystemCode")]
        [JsonConverter(typeof(ReferenceValueConverter), new object[] { "ava_erpsystems" })]
        public Guid? ERPSystemCodeID { get; set; }

        [JsonProperty("ava_ledgernumber", Required = Required.Default)]
        public string LedgerNumber { get; set; }

        [JsonProperty("ava_state", Required = Required.Default)]
        public string State { get; set; }

        [JsonProperty("ava_street1", Required = Required.Default)]
        public string Street1 { get; set; }

        [JsonProperty("ava_street2", Required = Required.Default)]
        public string Street2 { get; set; }

        [JsonProperty("ava_street3", Required = Required.Default)]
        public string Street3 { get; set; }

        [JsonProperty("ava_systemdescription", Required = Required.Default)]
        public string SystemDescription { get; set; }

        [JsonProperty("ava_systemid", Required = Required.Default)]
        public int SystemID { get; set; }

        [JsonProperty("ava_telephone", Required = Required.Default)]
        public string Telephone { get; set; }

        [JsonProperty("ava_zip", Required = Required.Default)]
        public string ZipCode { get; set; }
    }
}
