using Nov.Caps.Int.D365.Messaging;
using Nov.Caps.Int.D365.Messaging.Models;
using Nov.Caps.Int.D365.Services.Currencies;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Server.Handlers
{
    public class CurrencyExchangeHandler : IHandler<object, int>
    {
        private readonly ILogger logger;
        private readonly ExchangeService service;

        public CurrencyExchangeHandler(ILogger logger, ExchangeService service)
        {
            this.logger = logger;
            this.service = service;
        }

        public async Task<int> Handle(Payload<object> input)
        {
            await this.service.UpdateRates();

            return 0;
        }
    }
}
