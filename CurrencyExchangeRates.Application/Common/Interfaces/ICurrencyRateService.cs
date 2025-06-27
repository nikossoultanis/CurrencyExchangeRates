using CurrencyExchangeRates.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Application.Common.Interfaces
{
    public interface ICurrencyRateService
    {
        Task<CurrencyRate?> GetLatestRateAsync(string currencyCode);
        void SetRatesAsync(IEnumerable<CurrencyRate> rates);
    }
}
