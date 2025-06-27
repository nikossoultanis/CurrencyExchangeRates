using CurrencyExchangeRates.Application.Common.Jobs.Implementaions;
using CurrencyExchangeRates.Application.Common.Jobs.Interfaces;
using CurrencyExchangeRates.EcbGateway.Services.Implementations;
using CurrencyExchangeRates.EcbGateway.Services.Interfaces;
using CurrencyExchangeRates.Infrastructure.Jobs;
using CurrencyExchangeRates.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace CurrencyExchangeRates.Infrastructure.DepedencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register ECB Gateway
            services.AddHttpClient<IEcbRatesGatewayService, EcbRatesGatewayService>();

            services.AddScoped<ICurrencyRatesUpdateJob, CurrencyRatesUpdateJob>();

            services.AddQuartz(q =>
            {
                var jobKey = new JobKey("CurrencyRatesUpdateJob");

                q.ScheduleJob<QuartzCurrencyRatesUpdateJob>(trigger => trigger
                    .WithIdentity("CurrencyRatesUpdateTrigger")
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever()));
            });

            services.AddQuartzHostedService();
            return services;
        }
    }
}
