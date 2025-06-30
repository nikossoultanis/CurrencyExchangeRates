using CurrencyExchangeRates.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Application.Common.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet> CreateWalletAsync(string currency);
        Task<decimal> GetBalanceAsync(long walletId, string? requestedCurrency = null);
        Task AdjustBalanceAsync(long walletId, decimal amount, string currency, string strategyName);
    }
}
