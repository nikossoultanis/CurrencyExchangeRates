using CurrencyExchangeRates.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Application.Common.CQRS.Commands.Adjust
{
    public class AdjustWalletBalanceCommand : IRequest
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Strategy { get; set; }
    }
}
