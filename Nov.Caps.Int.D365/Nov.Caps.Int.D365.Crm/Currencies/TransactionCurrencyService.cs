using Nov.Caps.Int.D365.Crm.Core;
using System;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Crm.Currencies
{
    public class TransactionCurrencyService
    {
        private static readonly string ApiPath = "api/data/v9.1/transactioncurrencies";

        private readonly ApiClient api;

        public TransactionCurrencyService(ApiClient api)
        {
            this.api = api;
        }

        public Task<TransactionCurrencyEntity[]> FindAsync()
        {
            return this.api.GetAsync<TransactionCurrencyEntity[]>(TransactionCurrencyService.ApiPath);
        }

        public Task<TransactionCurrencyEntity[]> FindByCodeAsync(string code)
        {
            return this.api.GetAsync<TransactionCurrencyEntity[]>($"{TransactionCurrencyService.ApiPath}?$filter=isocurrencycode eq '{code}'");
        }

        public async Task UpdateAsync(TransactionCurrencyEntity currency)
        {
            await this.api.PatchAsync<TransactionCurrencyEntity, object>($"{TransactionCurrencyService.ApiPath}({currency.ID})", currency);
        }

        public async Task<Guid> CreateAsync(TransactionCurrencyEntity currency)
        {
            var str = await this.api.PostAsync<TransactionCurrencyEntity, string>(TransactionCurrencyService.ApiPath, currency);

            return Guid.Parse(str);
        }
    }
}
