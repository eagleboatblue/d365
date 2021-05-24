using System;
using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core;

namespace Nov.Caps.Int.D365.Crm.ProductGroup
{
    public class ProductGroupService
    {
        private static readonly string ApiPath = "api/data/v9.1/ava_productgroups";
        private readonly ApiClient api;
        public ProductGroupService(ApiClient api)
        {
            this.api = api;
        }
        public Task<ProductGroupEntity[]> FindByNameAsync(string name, Guid? businessUnit)
        {
            return this.api.GetAsync<ProductGroupEntity[]>($"{ProductGroupService.ApiPath}?$filter=ava_name eq '{name}' and _ava_businessunitid_value eq '{businessUnit}'");
        }
    }
}
