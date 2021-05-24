using Nov.Caps.Int.D365.Crm.Currencies;
using Nov.Caps.Int.D365.Data.Abstract;
using Serilog;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Nov.Caps.Int.D365.Services.Currencies
{
    public class ExchangeService
    {
        private readonly ILogger logger;
        private readonly ICurrencyRepository currencyRepository;
        private readonly TransactionCurrencyService currencyService;

        public ExchangeService(ILogger logger, ICurrencyRepository currencyRepository, TransactionCurrencyService currencyService)
        {
            this.logger = logger.ForContext("service", "currency_exchange");
            this.currencyRepository = currencyRepository;
            this.currencyService = currencyService;
        }

        public async Task UpdateRates()
        {
            this.logger.Information($"Starting to update rates...");

            var crmCurrencies = await this.currencyService.FindAsync();
            var map = crmCurrencies.ToDictionary(c => c.Code.ToLowerInvariant());

            this.logger.Information($"Found {map.Count} currencies in CRM");

            var incortaCurrencies = await this.currencyRepository.FindByCodes(
                map.Keys.ToArray(),
                DateTime.Now.ToUniversalTime()
            );

            this.logger.Information($"Found {incortaCurrencies.Count()} currencies in Incorta");

            var enumerator = incortaCurrencies.GetEnumerator();
            var errors = new Dictionary<string, Exception>();
            var count = 0;

            while (enumerator.MoveNext())
            {
                count++;
                var incortaCurrency = enumerator.Current;
                var crmCurrency = map[incortaCurrency.Code.ToLowerInvariant()];

                try
                {
                    crmCurrency.Rate = 1 / incortaCurrency.Rate;

                    await this.currencyService.UpdateAsync(crmCurrency);

                    this.logger.Information("Successfully update rate for {code} {id}", incortaCurrency.Code, crmCurrency.ID);
                }
                catch (Exception ex)
                {
                    errors[incortaCurrency.Code] = ex;

                    this.logger.Information("Failed to update rate for {code} {id} {@error}", incortaCurrency.Code, crmCurrency.ID, ex);
                }
            }

            if (errors.Keys.Count == 0)
            {
                this.logger.Information($"Successfully updated rates");

                return;
            }

            if (errors.Keys.Count == count)
            {
                this.logger.Information($"Failed to updated rates");
            }
            else
            {
                this.logger.Information("Updated rates with errors");
            }

            throw new ExchangeException(errors);
        }
    }
}
