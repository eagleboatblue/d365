using System;
using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core;

namespace Nov.Caps.Int.D365.Crm.Systems
{
    public class ErpSystemService
    {
        private static readonly string ApiPath = "api/data/v9.1/ava_erpsystems";
        private readonly ApiClient api;

        public ErpSystemService(ApiClient api)
        {
            this.api = api;
        }

        public Task<ErpSystemEntity[]> FindByExternalIDAsync(int id)
        {
            return this.api.GetAsync<ErpSystemEntity[]>($"{ErpSystemService.ApiPath}?$filter=ava_systemid eq '{id}'");
        }

        public Task<ErpSystemEntity[]> FindByDescrAsync(string descr)
        {
            return this.api.GetAsync<ErpSystemEntity[]>($"{ErpSystemService.ApiPath}?$filter=ava_systemdesc eq '{descr}'");
        }

        public async Task<Guid> CreateAsync(ErpSystemEntity erpSystem)
        {
            var str = await this.api.PostAsync<ErpSystemEntity, string>(ErpSystemService.ApiPath, erpSystem);

            return Guid.Parse(str);
        }
    }
}
