using CurrencyExchangeRates.EcbGateway.Services.Implementations;
using System.Net;

namespace CurrencyExchangeRates.Tests
{
    public class EcbGatewayTests
    {
        [Fact]
        public async Task GetDailyRatesAsync_ShouldParseRatesCorrectly()
        {
            // Arrange
            var fakeXml = @"
            <gesmes:Envelope xmlns:gesmes='http://www.gesmes.org/xml/2002-08-01'
                             xmlns='http://www.ecb.int/vocabulary/2002-08-01/eurofxref'>
                <Cube>
                    <Cube time='2023-06-01'>
                        <Cube currency='USD' rate='1.10'/>
                        <Cube currency='GBP' rate='0.85'/>
                    </Cube>
                </Cube>
            </gesmes:Envelope>";

            var handler = new FakeHttpMessageHandler(fakeXml);
            var httpClient = new HttpClient(handler);

            var gateway = new EcbRatesGateway(httpClient);

            // Act
            var rates = await gateway.GetDailyRatesAsync();

            // Assert
            Assert.Contains(rates, r => r.CurrencyCode == "USD" && r.Rate == 1.10m);
            Assert.Contains(rates, r => r.CurrencyCode == "GBP" && r.Rate == 0.85m);
        }
    }
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;

        public FakeHttpMessageHandler(string response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(_response)
            });
        }
    }
}