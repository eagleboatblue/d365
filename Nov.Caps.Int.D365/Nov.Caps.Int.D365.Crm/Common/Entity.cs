using Newtonsoft.Json;
using System;
namespace Nov.Caps.Int.D365.Crm.Common
{
    public class Entity
    {
        //[JsonProperty("createdby", Required = Required.Default)]
        //public Guid CreatedByID { get; set; }

        //[JsonProperty("createdon", Required = Required.Default)]
        //public DateTime CreatedOn { get; set; }

        //[JsonProperty("createdonbehalfby", Required = Required.Default)]
        //public Guid CreatedOnBehalfByID { get; set; }

        //[JsonProperty("modifiedby", Required = Required.Default)]
        //public Guid ModifiedByID { get; set; }

        //[JsonProperty("modifiedon", Required = Required.Default)]
        //public DateTime ModifiedOn { get; set; }

        //[JsonProperty("modifiedonbehalfby", Required = Required.Default)]
        //public Guid ModifiedOnBehalfByID { get; set; }

        //[JsonProperty("organizationid", Required = Required.Default)]
        //public Guid OrganizationID { get; set; }

        [JsonProperty("statecode", Required = Required.Default)]
        public int Status { get; set; }

        [JsonProperty("statuscode", Required = Required.Default)]
        public int StatusReason { get; set; }
    }
}
