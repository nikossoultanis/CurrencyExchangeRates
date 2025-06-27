using CurrencyExchangeRates.Domain.WalletRepository;
using CurrencyExchangeRates.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Infrastructure.Repository
{
    internal class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _dbContext;

        public WalletRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Domain.Entities.Wallet> CreateAsync(Domain.Entities.Wallet wallet)
        {
            _dbContext.Wallets.Add(wallet);
            await _dbContext.SaveChangesAsync();
            return wallet;
        }

        public async Task<Domain.Entities.Wallet?> GetByIdAsync(long id)
        {
            return await _dbContext.Wallets.FindAsync(id);
        }

        public async Task UpdateAsync(Domain.Entities.Wallet wallet)
        {
            _dbContext.Wallets.Update(wallet);
            await _dbContext.SaveChangesAsync();
        }
    }
}
