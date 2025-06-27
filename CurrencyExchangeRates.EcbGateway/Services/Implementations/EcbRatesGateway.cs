using CurrencyExchangeRates.EcbGateway.Models;
using CurrencyExchangeRates.EcbGateway.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CurrencyExchangeRates.EcbGateway.Services.Implementations
{
    public class EcbRatesGateway : IEcbRatesGateway
    {
        private readonly HttpClient _httpClient;
        private const string EcbUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        public EcbRatesGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CurrencyRate>> GetDailyRatesAsync()
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
                    Currency = x.Attribute("currency")?.Value,
                    Rate = decimal.Parse(
                        x.Attribute("rate")?.Value ?? "0",
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture),
                    Date = date
                })
                .ToList();

            return rates;
        }
    }
}
