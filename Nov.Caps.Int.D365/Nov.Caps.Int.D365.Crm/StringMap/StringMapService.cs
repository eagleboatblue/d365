using System;
using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core;

namespace Nov.Caps.Int.D365.Crm.StringMap
{
    public class StringMapService
    {
        private static readonly string ApiPath = "api/data/v9.1/stringmaps";
        private readonly ApiClient api;
        public StringMapService(ApiClient api)
        {
            this.api = api;
        }
        public Task<StringMapEntity[]> FindByValueAsync(string value, string entity, string attribute)
        {
            return this.api.GetAsync<StringMapEntity[]>($"{StringMapService.ApiPath}?$filter=value eq '{value}' and objecttypecode eq '{entity}' and attributename eq '{attribute}'");
        }        
    }
}
