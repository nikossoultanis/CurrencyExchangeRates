using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Application.Common.CQRS.Commands.Create
{
    internal class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
    {
        public CreateWalletCommandValidator()
        {
            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required.")
                .Length(3).WithMessage("Currency code must be exactly 3 characters.")
                .Matches("^[A-Z]{3}$").WithMessage("Currency code must be uppercase letters.");
        }
    }
}
