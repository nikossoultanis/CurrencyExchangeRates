using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Application.Common.Interfaces
{
    public interface ICurrencyGatewayFactory
    {
        ICurrencyGateway GetGateway(string providerName);
    }
}




