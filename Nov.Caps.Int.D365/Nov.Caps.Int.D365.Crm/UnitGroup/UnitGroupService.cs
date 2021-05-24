using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core;

namespace Nov.Caps.Int.D365.Crm.UnitGroup
{
    public class UnitGroupService
    {
        private static readonly string ApiPath = "api/data/v9.1/uomschedules";
        private readonly ApiClient api;

        public UnitGroupService(ApiClient api)
        {
            this.api = api;
        }

        public Task<UnitGroupEntity[]> FindByNameAsync(string name)
        {
            return this.api.GetAsync<UnitGroupEntity[]>($"{UnitGroupService.ApiPath}?$filter=name eq '{name}'");
        }
    }
}
