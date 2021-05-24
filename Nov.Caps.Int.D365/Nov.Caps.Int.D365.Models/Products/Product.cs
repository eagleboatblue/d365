using System;
using Nov.Caps.Int.D365.Models.Common;

namespace Nov.Caps.Int.D365.Models.Products
{
    public class Product
    {
		public string ProductLine { get; set; }

		public string BusinessUnit { get; set; }

		public string ProductGroup { get; set; }

		public string MinorCategory1 { get; set; }

		public string MinorCategory2 { get; set; }

		public string PartNumber { get; set; }

		public string MfgId { get; set; }

		public string ErpId { get; set; }

		public int State { get; set; }	

		public int Status { get; set; }

		public string ActiveSellable { get; set; }

		public string ShortDesc { get; set; }

		public string SalesDesc { get; set; }

		public string WhCode { get; set; }

		public string CatName { get; set; }

		public string DefaultUnit { get; set; }

		public string GrpA { get; set; }

		public string GrpB { get; set; }

		public string GrpC { get; set; }  

        public string UnitGroup { get; set; }		

		public string DefaultPriceList { get; set; }

		public int Quantity { get; set; }

		public bool Writein { get; set; }
		
		public Guid? ProductLineID { get; set; }

		public Guid? BusinessUnitID { get; set; }

		public Guid? ProductGroupID { get; set; }

		public Guid? MinorCategory1ID { get; set; }

		public Guid? MinorCategory2ID { get; set; }
		
		public Guid? UnitGroupID { get; set; }

	}
}