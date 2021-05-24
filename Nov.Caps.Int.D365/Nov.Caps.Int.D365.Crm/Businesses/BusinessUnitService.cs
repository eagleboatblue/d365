using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core;

namespace Nov.Caps.Int.D365.Crm.Businesses
{
    public class BusinessUnitService
    {
        private static readonly string ApiPath = "api/data/v9.1/businessunits";
        private readonly ApiClient api;

        public BusinessUnitService(ApiClient api)
        {
            this.api = api;
        }

        public Task<BusinessUnitEntity[]> FindByNameAsync(string name)
        {
            return this.api.GetAsync<BusinessUnitEntity[]>($"{BusinessUnitService.ApiPath}?$filter=name eq '{name}'");
        }
    }
}
