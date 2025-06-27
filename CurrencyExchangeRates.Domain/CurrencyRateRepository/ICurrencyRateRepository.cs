using CurrencyExchangeRates.Domain.Entities;

namespace CurrencyExchangeRates.Domain.CurrencyRateRepository
{
    public interface ICurrencyRateRepository
    {
        Task<CurrencyRate?> GetLatestRateAsync(string currency);
    }
}
