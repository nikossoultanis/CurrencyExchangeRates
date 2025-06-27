using CurrencyExchangeRates.Domain.Entities;

namespace CurrencyExchangeRates.Domain.CurrencyRateRepository
{
    public interface ICurrencyRateRepository
    {
        Task<List<CurrencyRate>> GetLatestRatesAsync();
        Task<CurrencyRate?> GetCurrencyRateAsync(string currency);
    }
}
