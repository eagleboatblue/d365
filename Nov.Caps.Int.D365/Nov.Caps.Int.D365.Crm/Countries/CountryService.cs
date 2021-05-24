using Nov.Caps.Int.D365.Crm.Core;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Crm.Countries
{
    public class CountryService
    {
        private static readonly string ApiPath = "api/data/v9.1/nov_country";
        private readonly ApiClient api;

        public CountryService(ApiClient api)
        {
            this.api = api;
        }

        public Task<CountryEntity[]> FindByNameAsync(string name)
        {
            return this.api.GetAsync<CountryEntity[]>($"{CountryService.ApiPath}?$filter=nov_name eq '{name}'");
        }
    }
}
