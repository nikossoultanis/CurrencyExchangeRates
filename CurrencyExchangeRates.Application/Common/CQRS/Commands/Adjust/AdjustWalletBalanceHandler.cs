using CurrencyExchangeRates.Application.Common.CQRS.Commands.Create;
using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Application.Common.CQRS.Commands.Adjust
{
    public class AdjustWalletBalanceHandler : IRequestHandler<AdjustWalletBalanceCommand>
    {
        private readonly IWalletService _walletService;

        public AdjustWalletBalanceHandler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task Handle(AdjustWalletBalanceCommand request, CancellationToken cancellationToken)
        {
            await _walletService.AdjustBalanceAsync(request.Id, request.Amount, request.Currency, request.Strategy);
        }
    }
}
