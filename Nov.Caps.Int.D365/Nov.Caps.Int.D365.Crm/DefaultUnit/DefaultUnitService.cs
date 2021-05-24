using System;
using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core;

namespace Nov.Caps.Int.D365.Crm.DefaultUnit
{
    public class DefaultUnitService
    {
        private static readonly string ApiPath = "api/data/v9.1/uoms";
        private readonly ApiClient api;
        public DefaultUnitService(ApiClient api)
        {
            this.api = api;
        }
        public Task<DefaultUnitEntity[]> FindByDefaultUnitAsync(string defaultUnit, Guid? unitGroup)
        {
            return this.api.GetAsync<DefaultUnitEntity[]>($"{DefaultUnitService.ApiPath}?$filter=name eq '{defaultUnit}' and _uomscheduleid_value eq '{unitGroup}'");
        }
        public async Task CreateAsync(DefaultUnitEntity defaultUnit)
        {
            await this.api.PostAsync<DefaultUnitEntity, object>(DefaultUnitService.ApiPath, defaultUnit);
        }
    }
}
