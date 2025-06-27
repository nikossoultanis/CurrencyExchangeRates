using CurrencyExchangeRates.Domain.Entities;

namespace CurrencyExchangeRates.Domain.Repositories
{
    public interface ICurrencyRateRepository
    {
        Task<List<CurrencyRate>> GetLatestRatesAsync();
        Task<CurrencyRate?> GetCurrencyRateAsync(string currency);
    }
}
