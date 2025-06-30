using FluentValidation;


namespace CurrencyExchangeRates.Application.Common.CQRS.Queries
{
    public class GetWalletBalanceQueryValidator : AbstractValidator<GetWalletBalanceQuery>
    {
        public GetWalletBalanceQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("WalletId must be greater than zero.");

            When(x => x.Currency != null, () =>
            {
                RuleFor(x => x.Currency)
                    .NotEmpty().WithMessage("Currency cannot be empty if provided.");
            });
        }
    }
}
