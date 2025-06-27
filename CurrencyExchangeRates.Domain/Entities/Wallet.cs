using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Domain.Entities
{
    public class Wallet
    {
        public long Id { get; private set; }
        public decimal Balance { get; private set; }
        public string Currency { get; private set; } = string.Empty;

        public Wallet(string currency, decimal initialBalance)
        {
            if (initialBalance < 0)
                throw new ArgumentException("Initial balance cannot be negative.");

            Currency = currency;
            Balance = initialBalance;
        }

        public void AddFunds(decimal amount)
        {
            Balance += amount;
        }

        public void SubtractFunds(decimal amount)
        {
            if (Balance < amount)
                throw new InvalidOperationException("Insufficient funds.");
            Balance -= amount;
        }

        public void ForceSubtractFunds(decimal amount)
        {
            Balance -= amount;
        }
    }
}
