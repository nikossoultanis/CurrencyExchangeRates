using CurrencyExchangeRates.Domain.WalletStrategies;

namespace CurrencyExchangeRates.Domain.Entities
{
    public class Wallet
    {
        public long Id { get; private set; }
        public decimal Balance { get; private set; }
        public string Currency { get; private set; } = string.Empty;

        public Wallet(string currency)
        {
            Currency = currency;
            Balance = 0;
        }

        public void ApplyStrategy(IWalletAdjustmentStrategy strategy, decimal amount)
        {
            strategy.Adjust(this, amount);
        }

        internal void IncreaseBalance(decimal amount)
        {
            Balance += amount;
        }

        internal void DecreaseBalance(decimal amount)
        {
            Balance -= amount;
        }
    }
}
