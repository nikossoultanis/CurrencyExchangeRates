using CurrencyExchangeRates.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Domain.Repositories
{
    public interface IWalletRepository
    {
        Task<Wallet> CreateAsync(Wallet wallet);
        Task<Wallet?> GetByIdAsync(long id);
        Task UpdateAsync(Wallet wallet);
    }
}
