using System;
using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core;

namespace Nov.Caps.Int.D365.Crm.ProductLine
{
    public class ProductLineService
    {
        private static readonly string ApiPath = "api/data/v9.1/ava_productlines";
        private readonly ApiClient api;

        public ProductLineService(ApiClient api)
        {
            this.api = api;
        }

        public Task<ProductLineEntity[]> FindByProductLineAsync(string prodLine, Guid? businessUnit, Guid? productGroup)
        {
            return this.api.GetAsync<ProductLineEntity[]>($"{ProductLineService.ApiPath}?$filter=ava_name eq '{prodLine}' and _ava_businessunit_value eq '{businessUnit}' and _ava_productgroupid_value eq '{productGroup}'");
        }
        
        public async Task CreateAsync(ProductLineEntity productLine)
        {
            await this.api.PostAsync<ProductLineEntity, string>(ProductLineService.ApiPath, productLine);

        }
    }
}
