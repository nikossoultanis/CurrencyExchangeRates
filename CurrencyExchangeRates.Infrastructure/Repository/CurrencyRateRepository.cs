using CurrencyExchangeRates.Domain.Entities;
using CurrencyExchangeRates.Domain.Repositories;
using CurrencyExchangeRates.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeRates.Infrastructure.Repository
{
    public class CurrencyRateRepository : ICurrencyRateRepository
    {
        private readonly AppDbContext _dbContext;

        public CurrencyRateRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public  async Task<List<CurrencyRate>> GetLatestRatesAsync()
        {
            return await _dbContext.CurrencyRates.ToListAsync();
        }

        public async Task<CurrencyRate?> GetCurrencyRateAsync(string currency)
        {
            return await _dbContext.CurrencyRates
                .FirstOrDefaultAsync(r => r.CurrencyCode == currency);
        }
    }
}
