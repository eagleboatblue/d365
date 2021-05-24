using System;
namespace Nov.Caps.Int.D365.Models.ERPSystems
{
    public class ERPSystem
    {
        public int ID { get; set; }

        public string Description { get; set; }

        public int WK { get; set; }

        public string ParentSystem { get; set; }

        public int DefaultBusinessUnitID { get; set; }

        public int BusinessWK { get; set; }

        public string DefaultLedgerNumber { get; set; }

        public string DefaultLedgerDescription { get; set; }

        public int ActiveERP { get; set; }

        public string BusinessName { get; set; }

        public string BusinessUnitCode { get; set; }

        public string BusinessUnitDescription { get; set; }

        public string ITContact { get; set; }

        public string Notes { get; set; }
    }
}
