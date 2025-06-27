
using CurrencyExchangeRates.Domain.WalletStrategies;

namespace CurrencyExchangeRates.Application.Common.Interfaces
{
    public interface IWalletAdjustmentStrategyFactory
    {
        IWalletAdjustmentStrategy GetStrategy(string strategyName);
    }
}
