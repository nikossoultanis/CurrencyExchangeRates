using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Domain.Entities;
using CurrencyExchangeRates.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;


namespace CurrencyExchangeRates.Infrastructure.Services
{
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly IMemoryCache _cache;
        private readonly ICurrencyRateRepository _currencyRateRepository;

        private static readonly string CacheKey = "LatestCurrencyRates";

        public CurrencyRateService(
            IMemoryCache cache,
            ICurrencyRateRepository currencyRateRepository)
        {
            _cache = cache;
            _currencyRateRepository = currencyRateRepository;
        }

        public async Task<CurrencyRate?> GetLatestRateAsync(string currencyCode)
        {
            var rates = await GetAllRatesAsync();

            rates.TryGetValue(currencyCode.ToUpperInvariant(), out var rate);

            return rate;
        }

        public void SetRatesAsync(IEnumerable<CurrencyRate> rates)
        {
            var dict = rates.ToDictionary(
                r => r.CurrencyCode.ToUpperInvariant(),
                r => r,
                StringComparer.OrdinalIgnoreCase);

            // Store in memory cache
            _cache.Set(CacheKey, dict, TimeSpan.FromHours(24));
        }

        private async Task<Dictionary<string, CurrencyRate>> GetAllRatesAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out Dictionary<string, CurrencyRate>? ratesDict))
            {
                var ratesFromDb = await _currencyRateRepository.GetLatestRatesAsync();

                ratesDict = ratesFromDb.ToDictionary(
                    r => r.CurrencyCode.ToUpperInvariant(),
                    r => r,
                    StringComparer.OrdinalIgnoreCase);

                _cache.Set(CacheKey, ratesDict, TimeSpan.FromHours(24));
            }

            return ratesDict;
        }
    }
}
