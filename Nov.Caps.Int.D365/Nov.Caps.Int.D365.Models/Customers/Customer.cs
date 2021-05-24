using Nov.Caps.Int.D365.Models.Common;

namespace Nov.Caps.Int.D365.Models.Customers
{
    public class Customer
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Telephone { get; set; }

        public string LedgerNumber { get; set; }

        public string DefaultPaymentCode { get; set; }

        public int DWCustomerID { get; set; }

        public decimal DWSource { get; set; }

        public int SystemID { get; set; }

        public string SystemDescription { get; set; }

        public decimal CreditLimit { get; set; }

        public string CurrencyCode { get; set; }

        public Address Address { get; set; }
    }
}
