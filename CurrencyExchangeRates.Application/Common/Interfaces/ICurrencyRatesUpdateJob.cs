﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Application.Common.Interfaces
{
    public interface ICurrencyRatesUpdateJob
    {
        Task ExecuteAsync(CancellationToken cancellationToken, string providerName = "ECB");
    }
}
