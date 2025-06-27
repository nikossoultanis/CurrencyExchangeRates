using CurrencyExchangeRates.Domain.Entities;

namespace CurrencyExchangeRates.Domain.WalletStrategies
{
    public class SubtractFundsStrategy : IWalletAdjustmentStrategy
    {
        public void Adjust(Wallet wallet, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            if (wallet.Balance < amount)
                throw new InvalidOperationException("Insufficient funds.");

            wallet.DecreaseBalance(amount);
        }
    }
}
