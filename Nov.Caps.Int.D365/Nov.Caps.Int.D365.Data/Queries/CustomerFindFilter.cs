using System.Collections.Generic;

namespace Nov.Caps.Int.D365.Data.Queries
{
    public class CustomerFindFilter
    {
        public List<string> LedgerNumber { get; set; }

        public List<int> SystemID { get; set; }

        public List<string> CustomerCode { get; set; }
    }
}
