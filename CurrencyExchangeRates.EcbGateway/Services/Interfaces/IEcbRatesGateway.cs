using CurrencyExchangeRates.EcbGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.EcbGateway.Services.Interfaces
{
    public interface IEcbRatesGateway
    {
        Task<IEnumerable<CurrencyRate>> GetDailyRatesAsync();
    }
}
