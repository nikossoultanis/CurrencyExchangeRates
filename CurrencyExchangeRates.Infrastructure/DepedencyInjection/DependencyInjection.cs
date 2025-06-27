using CurrencyExchangeRates.EcbGateway.Services.Implementations;
using CurrencyExchangeRates.EcbGateway.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyExchangeRates.Infrastructure.DepedencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Register ECB Gateway
            services.AddHttpClient<IEcbRatesGateway, EcbRatesGateway>();

            return services;
        }
    }
}
