using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nov.Caps.Int.D365.Messaging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.Threading;

namespace Nov.Caps.Int.D365.Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Filter.ByExcluding("EndsWith(RequestPath, '/health')")
                .WriteTo.Console(new CompactJsonFormatter())
                .CreateLogger();
            Log.Logger = logger;

            services.AddSingleton<ILogger>(logger);
            services.AddHealthChecks();

            this.addDatabase(services);
            this.addCRM(services);
            this.addMessaging(services);
            this.addServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
            });

            // Messaging router
            var router = app.ApplicationServices.GetService<Router>();

            // Register handlers
            router.AddHandler("request.batch.currency_exchange", app.ApplicationServices.GetService<Handlers.CurrencyExchangeHandler>());
            router.AddHandler("request.batch.customer_accounts", app.ApplicationServices.GetService<Handlers.CustomerAccountsUpdate>());
            router.AddHandler("request.batch.products_update", app.ApplicationServices.GetService<Handlers.ProductsUpdateHandler>());

            var cancellation = new CancellationTokenSource();

            try
            {
                var lifecycle = app.ApplicationServices.GetService<IHostApplicationLifetime>();

                lifecycle.ApplicationStopping.Register(cancellation.Cancel);
                router.Start(cancellation.Token);
            }
            catch (Exception e)
            {
                cancellation.Cancel();
                System.Console.WriteLine(e);
                Environment.Exit(1);
            }
        }

        private void addDatabase(IServiceCollection services)
        {
            services.AddSingleton(new Data.Common.ConnectionSettings(
                Environment.GetEnvironmentVariable("INCORTA_SERVER"),
                Environment.GetEnvironmentVariable("INCORTA_PORT"),
                Environment.GetEnvironmentVariable("INCORTA_DATABASE"),
                Environment.GetEnvironmentVariable("INCORTA_USER"),
                Environment.GetEnvironmentVariable("INCORTA_PASSWORD"),
                90000,
                90000
            ));

            services.AddTransient<Data.Abstract.ICurrencyRepository, Data.Incorta.CurrencyRepository>();
            services.AddTransient<Data.Abstract.ICustomerRepository, Data.Incorta.CustomerRepository>();
            services.AddTransient<Data.Abstract.IERPSystemRepository, Data.Incorta.ERPSystemRepository>();
            services.AddTransient<Data.Abstract.IProductRepository, Data.Incorta.ProductRepository>();
        }

        private void addCRM(IServiceCollection services)
        {
            services.AddSingleton(new Crm.Core.Auth.TokenProviderSettings()
            {
                InstanceUri = new Uri(Environment.GetEnvironmentVariable("D365_URL")),
                AdfsUri = new Uri(Environment.GetEnvironmentVariable("D365_ADFS")),
                Authority = Environment.GetEnvironmentVariable("D365_AUTHORITY"),
                ClientID = Environment.GetEnvironmentVariable("D365_CLIENT_ID"),
                ClientSecret = Environment.GetEnvironmentVariable("D365_CLIENT_SECRET"),
                Username = Environment.GetEnvironmentVariable("D365_USER"),
                Password = Environment.GetEnvironmentVariable("D365_PASSWORD")
            });

            services.AddTransient<System.Net.Http.HttpClient>();
            services.AddTransient<Crm.Core.Auth.TokenProvider>();
            services.AddTransient<Crm.Core.ApiClient>();
            services.AddTransient<Crm.Accounts.ErpAccountService>();
            services.AddTransient<Crm.Businesses.BusinessUnitService>();
            services.AddTransient<Crm.Currencies.TransactionCurrencyService>();
            services.AddTransient<Crm.Systems.ErpSystemService>();
            services.AddTransient<Crm.Systems.ErpParentSystemService>();
            services.AddTransient<Crm.Countries.CountryService>();
            services.AddTransient<Crm.Products.ErpProductService>();
            services.AddTransient<Crm.ProductGroup.ProductGroupService>();
            services.AddTransient<Crm.ProductLine.ProductLineService>();
            services.AddTransient<Crm.MinorCategory1.MinorCategory1Service>();
            services.AddTransient<Crm.MinorCategory2.MinorCategory2Service>();
            services.AddTransient<Crm.UnitGroup.UnitGroupService>();
            services.AddTransient<Crm.DefaultUnit.DefaultUnitService>();
            services.AddTransient<Crm.StringMap.StringMapService>();
            services.AddTransient<Crm.PriceLevel.PriceLevelService>();
            services.AddTransient<Crm.PriceListItems.PriceListItemsService>();
        }

        private void addMessaging(IServiceCollection services)
        {
            services.AddSingleton(new Settings(
                Environment.GetEnvironmentVariable("RABBITMQ_URL"),
                Environment.GetEnvironmentVariable("RABBITMQ_USER"),
                Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")
            ));

            // Messaging router
            services.AddTransient<Router>();

            // Message handlers
            services.AddTransient<Handlers.CurrencyExchangeHandler>();
            services.AddTransient<Handlers.CustomerAccountsUpdate>();
            services.AddTransient<Handlers.ProductsUpdateHandler>();
        }

        private void addServices(IServiceCollection services)
        {
            services.AddTransient<Services.Currencies.ExchangeService>();
            services.AddTransient<Services.Accounts.SyncService>();
            services.AddTransient<Services.Products.ProductSyncService>();
        }
    }
}
