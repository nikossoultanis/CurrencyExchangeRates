using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Domain.Entities;
using MediatR;

namespace CurrencyExchangeRates.Application.Common.CQRS.Commands.Create
{
    public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, Wallet>
    {
        private readonly IWalletService _walletService;

        public CreateWalletCommandHandler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<Wallet> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
        {
            var wallet = await _walletService.CreateWalletAsync(request.Currency);
            return wallet;
        }
    }
}
