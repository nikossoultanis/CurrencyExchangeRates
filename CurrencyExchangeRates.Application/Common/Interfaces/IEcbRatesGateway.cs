using CurrencyExchangeRates.Domain.Entities;

namespace CurrencyExchangeRates.EcbGateway.Services.Interfaces
{
    public interface IEcbRatesGateway
    {
        Task<List<CurrencyRate>> GetDailyRatesAsync(CancellationToken cancellationToken = default);
    }
}
