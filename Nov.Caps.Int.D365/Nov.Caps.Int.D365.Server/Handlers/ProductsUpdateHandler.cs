using Nov.Caps.Int.D365.Messaging;
using Nov.Caps.Int.D365.Messaging.Models;
using Nov.Caps.Int.D365.Services.Products;
using Serilog;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Server.Handlers
{
    public class ProductsUpdateHandler : IHandler<object, int>
    {
        private readonly ILogger logger;
        private readonly ProductSyncService service;

        public ProductsUpdateHandler(ILogger logger, ProductSyncService service)
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
