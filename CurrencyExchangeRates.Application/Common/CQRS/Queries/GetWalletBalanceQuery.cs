using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Application.Common.CQRS.Queries
{
    public class GetWalletBalanceQuery : IRequest<decimal>
    {
        public long Id { get; set; }
        public string? Currency { get; set; }
    }
}
