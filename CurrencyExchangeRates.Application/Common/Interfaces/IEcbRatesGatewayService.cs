using CurrencyExchangeRates.Domain.Entities;

namespace CurrencyExchangeRates.EcbGateway.Services.Interfaces
{
    public interface IEcbRatesGatewayService
    {
        Task<List<CurrencyRate>> GetDailyRatesAsync(CancellationToken cancellationToken = default);
    }
}
