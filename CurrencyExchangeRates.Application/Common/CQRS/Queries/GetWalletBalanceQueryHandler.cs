using CurrencyExchangeRates.Application.Common.Interfaces;
using MediatR;

namespace CurrencyExchangeRates.Application.Common.CQRS.Queries
{
    public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, decimal>
    {
        private readonly IWalletService _walletService;

        public GetWalletBalanceQueryHandler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<decimal> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
        {
            var balance = await _walletService.GetBalanceAsync(request.Id, request.Currency);
            return balance;
        }
    }
}
