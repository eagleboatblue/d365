using System;
using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core;

namespace Nov.Caps.Int.D365.Crm.MinorCategory1
{
    public class MinorCategory1Service
    {
        private static readonly string ApiPath = "api/data/v9.1/ava_minorcategory1s";
        private readonly ApiClient api;

        public MinorCategory1Service(ApiClient api)
        {
            this.api = api;
        }

        public Task<MinorCategory1Entity[]> FindByMinorCategory1Async(string minorCategory1, Guid? businessUnit, Guid? productGroup, Guid? prodLine)
        {
            return this.api.GetAsync<MinorCategory1Entity[]>($"{MinorCategory1Service.ApiPath}?$filter=ava_name eq '{minorCategory1}' and _ava_businessunit_value eq '{businessUnit}' and _ava_productgroupid_value eq '{productGroup}' and _ava_productline_value eq '{prodLine}'");
        }

 
        public async Task CreateAsync(MinorCategory1Entity minorCategory1)
        {
            await this.api.PostAsync<MinorCategory1Entity, string>(MinorCategory1Service.ApiPath, minorCategory1);

        }
    }
}
