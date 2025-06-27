using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Application.WalletLogic
{
    public class ForceSubtractFundsStrategy : IFundsAdjustmentStrategy
    {
        public void AdjustBalance(decimal amount, ref decimal currentBalance)
        {
            currentBalance -= amount;
        }
    }
}
