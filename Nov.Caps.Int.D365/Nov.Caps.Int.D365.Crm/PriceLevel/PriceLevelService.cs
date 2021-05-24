using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core;

namespace Nov.Caps.Int.D365.Crm.PriceLevel
{
    public class PriceLevelService
    {
        private static readonly string ApiPath = "api/data/v9.1/pricelevels";
        private readonly ApiClient api;
        public PriceLevelService(ApiClient api)
        {
            this.api = api;
        }
        public Task<PriceLevelEntity[]> FindByNameAsync(string name)
        {
            return this.api.GetAsync<PriceLevelEntity[]>($"{PriceLevelService.ApiPath}?$filter=name eq '{name}'");
        }
    }
}
