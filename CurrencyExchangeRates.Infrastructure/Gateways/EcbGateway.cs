using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Domain.Entities;
using CurrencyExchangeRates.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Xml.Linq;

namespace CurrencyExchangeRates.Infrastructure.Gateways
{
    public class EcbGateway : ICurrencyGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _ecbUrl;
        public string GatewayProviderName => "ECB";

        public EcbGateway(HttpClient httpClient,
            IOptions<EcbGatewayOptions> options)
        {
            _httpClient = httpClient;
            _ecbUrl = options.Value.Url;
        }

        public async Task<List<CurrencyRate>> GetDailyRatesAsync(CancellationToken cancellationToken = default)
        {
            var xml = await _httpClient.GetStringAsync(_ecbUrl);

            var xdoc = XDocument.Parse(xml);
            var ns = xdoc.Root.GetDefaultNamespace();

            // Find the Cube with the time attribute
            var timeCube = xdoc.Descendants(ns + "Cube")
                               .FirstOrDefault(x => x.Attribute("time") != null);

            if (timeCube == null)
                throw new Exception("ECB XML does not contain expected time node.");

            var date = DateTime.Parse(timeCube.Attribute("time").Value);

            var rates = timeCube.Elements()
                .Select(x => new CurrencyRate
                {
                    CurrencyCode = x.Attribute("currency")?.Value,
                    Rate = decimal.Parse(
                        x.Attribute("rate")?.Value ?? "0",
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture),
                    Date = date
                })
                .ToList();

            rates.Add(new CurrencyRate
            {
                CurrencyCode = "EUR",
                Rate = 1.0m,
                Date = DateTime.UtcNow.Date
            });
            
            return rates;
        }
    }
}
