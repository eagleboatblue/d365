using System;
using System.Collections.Generic;
using System.Linq;

namespace Nov.Caps.Int.D365.Crm.Products
{
    public class ErpProductFilter
    {
        public string ProductNumber;
        public override string ToString()
        {
            var sb = new List<KeyValuePair<string, string>>();   
            if (!string.IsNullOrWhiteSpace(this.ProductNumber))
            {
                sb.Add(new KeyValuePair<string, string>("productnumber", this.ProductNumber));
            }

            var filterQuery = string.Join(" and ", sb.Select(i => $"{i.Key} eq '{i.Value}'"));

            if (!string.IsNullOrWhiteSpace(filterQuery))
            {
                return $"$filter={filterQuery}";
            }
            
            return string.Empty;
        }
    }
}
