using CurrencyExchangeRates.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CurrencyExchangeRates.Infrastructure.Gateways
{
    public class CurrencyGatewayFactory : ICurrencyGatewayFactory
    {
        private readonly IEnumerable<ICurrencyGateway> _gateways;
        private readonly ILogger<ICurrencyGatewayFactory> _logger;

        public CurrencyGatewayFactory(IEnumerable<ICurrencyGateway> gateways, ILogger<ICurrencyGatewayFactory> logger)
        {
            _gateways = gateways;
            _logger = logger;
        }

        ICurrencyGateway ICurrencyGatewayFactory.GetGateway(string providerName)
        {
            var gateway = _gateways.FirstOrDefault(g =>
                g.GatewayProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

            if (gateway == null)
            {
                _logger.LogWarning($"No gateway registered for provider '{providerName}'.");
                throw new Exception($"No gateway registered for provider '{providerName}'."); 
            }
            return gateway;
        }
    }
}
