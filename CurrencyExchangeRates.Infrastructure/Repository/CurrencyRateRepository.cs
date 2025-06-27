using CurrencyExchangeRates.Domain.CurrencyRateRepository;
using CurrencyExchangeRates.Domain.Entities;
using CurrencyExchangeRates.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Infrastructure.Repository
{
    public class CurrencyRateRepository : ICurrencyRateRepository
    {
        private readonly AppDbContext _dbContext;

        public CurrencyRateRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CurrencyRate?> GetLatestRateAsync(string currency)
        {
            return await _dbContext.CurrencyRates
                .FirstOrDefaultAsync(r => r.CurrencyCode == currency);
        }
    }
}
