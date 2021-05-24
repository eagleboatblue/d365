using System;
using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core;

namespace Nov.Caps.Int.D365.Crm.PriceListItems
{
    public class PriceListItemsService
    {
        private static readonly string ApiPath = "api/data/v9.1/productpricelevels";
        private readonly ApiClient api;
        public PriceListItemsService(ApiClient api)
        {
            this.api = api;
        }
        public async Task CreateAsync(PriceListItemsEntity priceListItem)
        {
            await this.api.PostAsync<PriceListItemsEntity, object>(PriceListItemsService.ApiPath, priceListItem);
        }
        public Task<PriceListItemsEntity[]> FindByNameAsync(Guid? name, Guid? product)
        {
            return this.api.GetAsync<PriceListItemsEntity[]>($"{PriceListItemsService.ApiPath}?$filter=_pricelevelid_value eq '{name}' and _productid_value eq '{product}'");
        }
    }
}
