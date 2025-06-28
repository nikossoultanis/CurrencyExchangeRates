using CurrencyExchangeRates.Domain.Entities;

namespace CurrencyExchangeRates.Application.Common.Interfaces
{
    public interface ICurrencyGateway
    {
        string GatewayProviderName { get; }
        Task<List<CurrencyRate>> GetDailyRatesAsync(CancellationToken cancellationToken = default);
    }
}
