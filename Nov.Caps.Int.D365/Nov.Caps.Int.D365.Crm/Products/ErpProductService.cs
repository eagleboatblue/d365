using Nov.Caps.Int.D365.Crm.Core;
using System;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Crm.Products
{
    public class ErpProductService
    {
        private static readonly string ApiPath = "api/data/v9.1/products";
        private readonly ApiClient api;
        public ErpProductService(ApiClient api)
        {
            this.api = api;
        }
        public Task<ErpProductEntity[]> FindBy(ErpProductFilter filter)
        {
            return this.api.GetAsync<ErpProductEntity[]>($"{ErpProductService.ApiPath}?{filter}");
        }		
        public async Task CreateAsync(ErpProductEntity product)
        {
            var str = await this.api.PostAsync<ErpProductEntity, string>(ErpProductService.ApiPath, product);
        }
        public async Task UpdateAsync(ErpProductEntity product)
        {
            await this.api.PatchAsync<ErpProductEntity, object>($"{ErpProductService.ApiPath}({product.ID})", product);
        }
    }
}
