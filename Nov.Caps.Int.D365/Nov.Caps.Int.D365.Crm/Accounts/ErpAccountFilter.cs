using System;
using System.Collections.Generic;
using System.Linq;

namespace Nov.Caps.Int.D365.Crm.Accounts
{
    public class ErpAccountFilter
    {
        public string SystemDescription;

        public string CustomerCode;

        public override string ToString()
        {
            var sb = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(this.SystemDescription))
            {
                sb.Add(new KeyValuePair<string, string>("ava_systemdescription", this.SystemDescription));
            }

            if (!string.IsNullOrWhiteSpace(this.CustomerCode))
            {
                sb.Add(new KeyValuePair<string, string>("ava_customercode", this.CustomerCode));
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
