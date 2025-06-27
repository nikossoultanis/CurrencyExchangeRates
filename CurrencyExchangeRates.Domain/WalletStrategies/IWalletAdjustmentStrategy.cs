using CurrencyExchangeRates.Domain.Entities;

namespace CurrencyExchangeRates.Domain.WalletStrategies
{
    public interface IWalletAdjustmentStrategy
    {
        void Adjust(Wallet wallet, decimal amount);
    }
}
