using Newtonsoft.Json;
using Nov.Caps.Int.D365.Crm.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Crm.Accounts
{
    public class ErpAccountService
    {
        private static readonly string ApiPath = "api/data/v9.1/ava_erpaccounts";
        private readonly ApiClient api;

        public ErpAccountService(ApiClient api)
        {
            this.api = api;
        }

        public Task<ErpAccountEntity[]> FindBy(ErpAccountFilter filter)
        {
            return this.api.GetAsync<ErpAccountEntity[]>($"{ErpAccountService.ApiPath}?{filter}");
        }

        public async Task CreateAsync(ErpAccountEntity account)
        {
            await this.api.PostAsync<ErpAccountEntity, object>(ErpAccountService.ApiPath, account);
        }

        public async Task UpdateAsync(ErpAccountEntity account)
        {
            await this.api.PatchAsync<ErpAccountEntity, object>($"{ErpAccountService.ApiPath}({account.ID})", account);
        }
    }
}
