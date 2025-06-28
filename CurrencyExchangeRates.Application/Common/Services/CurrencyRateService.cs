using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Domain.Entities;

namespace CurrencyExchangeRates.Application.Common.Services
{
    public class CurrencyRateService
    {
        private readonly IGatewayService _gateway;

        public CurrencyRateService(IGatewayService gateway)
        {
            _gateway = gateway;
        }

        public async Task<List<CurrencyRate>> GetRatesAsync()
        {
            return await _gateway.GetDailyRatesAsync();
        }
    }
}
