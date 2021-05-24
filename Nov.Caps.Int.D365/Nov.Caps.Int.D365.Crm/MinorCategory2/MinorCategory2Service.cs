using System;
using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core;

namespace Nov.Caps.Int.D365.Crm.MinorCategory2
{
    public class MinorCategory2Service
    {
        private static readonly string ApiPath = "api/data/v9.1/ava_minorcategory2s";
        private readonly ApiClient api;
        public MinorCategory2Service(ApiClient api)
        {
            this.api = api;
        }
        public Task<MinorCategory2Entity[]> FindByMinorCategory2Async(string minorCategory2, Guid? minorCategory1, Guid? businessUnit, Guid? productGroup, Guid? prodLine)
        {
            return this.api.GetAsync<MinorCategory2Entity[]>($"{MinorCategory2Service.ApiPath}?$filter=ava_name eq '{minorCategory2}' and _ava_minorcategory1_value eq '{minorCategory1}' and _ava_businessunit_value eq '{businessUnit}' and _ava_productgroupid_value eq '{productGroup}' and _ava_productline_value eq '{prodLine}'");
        }
        public async Task CreateAsync(MinorCategory2Entity minorCategory2)
        {            
           await this.api.PostAsync<MinorCategory2Entity, object>(MinorCategory2Service.ApiPath, minorCategory2);

        }
    }
}
