using System;
using System.Threading.Tasks;
using Nov.Caps.Int.D365.Crm.Core;

namespace Nov.Caps.Int.D365.Crm.Systems
{
    public class ErpParentSystemService
    {
        private static readonly string ApiPath = "api/data/v9.1/ava_erpparentsystem";
        private readonly ApiClient api;

        public ErpParentSystemService(ApiClient api)
        {
            this.api = api;
        }

        public Task<ErpParentSystemEntity[]> FindByName(string name)
        {
            return this.api.GetAsync<ErpParentSystemEntity[]>($"{ErpParentSystemService.ApiPath}?$filter=ava_name eq '{name}'");
        }
    }
}
