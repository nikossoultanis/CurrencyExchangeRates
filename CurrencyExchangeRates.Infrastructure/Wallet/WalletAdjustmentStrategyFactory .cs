using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Domain.WalletStrategies;

namespace CurrencyExchangeRates.Infrastructure.Wallet
{
    public class WalletAdjustmentStrategyFactory : IWalletAdjustmentStrategyFactory
    {
        public IWalletAdjustmentStrategy GetStrategy(string strategyName)
        {
            return strategyName switch
            {
                "AddFundsStrategy" => new AddFundsStrategy(),
                "SubtractFundsStrategy" => new SubtractFundsStrategy(),
                "ForceSubtractFundsStrategy" => new ForceSubtractFundsStrategy(),
                _ => throw new ArgumentException($"Unknown strategy: {strategyName}")
            };
        }
    }
}
