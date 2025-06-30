using FluentValidation;

namespace CurrencyExchangeRates.Application.Common.CQRS.Commands.Adjust
{
    public class AdjustWalletBalanceValidator : AbstractValidator<AdjustWalletBalanceCommand>
    {
        public AdjustWalletBalanceValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Currency).NotEmpty();
            RuleFor(x => x.Strategy).NotEmpty();
        }
    }
}
