using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Domain.Entities;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CurrencyExchangeRates.Infrastructure.Gateways
{
    internal class OpenExchangeGateway : ICurrencyGateway
    {
        private readonly HttpClient _httpClient;
        private readonly OpenExchangeOptions _options;

        public string GatewayProviderName => "OpenExchange";

        public OpenExchangeGateway(HttpClient httpClient, IOptions<OpenExchangeOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<List<CurrencyRate>> GetDailyRatesAsync(CancellationToken cancellationToken = default)
        {
            var url = $"{_options.BaseUrl}?app_id={_options.AppId}";

            var response = await _httpClient.GetAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var model = JsonSerializer.Deserialize<OpenExchangeResponse>(json);

            if (model == null)
                throw new Exception("Failed to parse OpenExchange API response.");

            var rates = model.Rates
                .Select(x => new CurrencyRate
                {
                    CurrencyCode = x.Key,
                    Rate = (decimal)x.Value,
                    Date = DateTime.UtcNow
                })
                .ToList();

            return rates;
        }
    }
    public class OpenExchangeResponse
    {
        public string Base { get; set; }
        public Dictionary<string, double> Rates { get; set; }
    }

    public class OpenExchangeOptions
    {
        public string BaseUrl { get; set; }
        public string AppId { get; set; }
    }
}
