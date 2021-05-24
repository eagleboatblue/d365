using Nov.Caps.Int.D365.Messaging;
using Nov.Caps.Int.D365.Messaging.Models;
using Nov.Caps.Int.D365.Services.Accounts;
using Serilog;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Server.Handlers
{
    public class CustomerAccountsUpdate : IHandler<object, int>
    {
        private readonly ILogger logger;
        private readonly SyncService service;

        public CustomerAccountsUpdate(ILogger logger, SyncService service)
        {
            this.logger = logger;
            this.service = service;
        }

        public async Task<int> Handle(Payload<object> input)
        {
            await this.service.Sync();

            return 0;
        }
    }
}
