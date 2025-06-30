using CurrencyExchangeRates.Domain.Entities;
using MediatR;

namespace CurrencyExchangeRates.Application.Common.CQRS.Commands.Create
{
    public class CreateWalletCommand : IRequest<Wallet>
    {
        public string Currency { get; set; }
    }
}
