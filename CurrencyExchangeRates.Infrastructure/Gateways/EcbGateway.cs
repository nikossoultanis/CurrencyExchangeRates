using CurrencyExchangeRates.Domain.Entities;
using CurrencyExchangeRates.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CurrencyExchangeRates.Infrastructure.Gateways
{
    public class EcbGateway : IGatewayService
    {
        private readonly HttpClient _httpClient;
        private const string EcbUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        public EcbGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CurrencyRate>> GetDailyRatesAsync(CancellationToken cancellationToken = default)
        {
            var xml = await _httpClient.GetStringAsync(EcbUrl);

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
