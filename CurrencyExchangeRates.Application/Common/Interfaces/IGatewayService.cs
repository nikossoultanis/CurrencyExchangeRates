using CurrencyExchangeRates.Domain.Entities;

namespace CurrencyExchangeRates.Application.Common.Interfaces
{
    public interface IGatewayService
    {
        Task<List<CurrencyRate>> GetDailyRatesAsync(CancellationToken cancellationToken = default);
    }
}
