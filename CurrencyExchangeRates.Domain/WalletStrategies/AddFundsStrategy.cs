using CurrencyExchangeRates.Domain.Entities;

namespace CurrencyExchangeRates.Domain.WalletStrategies
{
    public class AddFundsStrategy : IWalletAdjustmentStrategy
    {
        public void Adjust(Wallet wallet, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            wallet.IncreaseBalance(amount);
        }
    }
}
