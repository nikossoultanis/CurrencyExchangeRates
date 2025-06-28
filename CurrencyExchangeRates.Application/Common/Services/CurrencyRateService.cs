using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Domain.Entities;

namespace CurrencyExchangeRates.Application.Common.Services
{
    public class CurrencyRateService
    {
        private readonly ICurrencyGatewayFactory _gatewayFactory;

        public CurrencyRateService(ICurrencyGatewayFactory gateway)
        {
            _gatewayFactory = gateway;
        }

        public async Task<List<CurrencyRate>> GetRatesAsync(string providerName)
        {
            try
            {
                var gateway = _gatewayFactory.GetGateway(providerName);
                return await gateway.GetDailyRatesAsync();
            }
            catch (Exception ex)
            {
                return new List<CurrencyRate>();
            }

        }
    }
}
