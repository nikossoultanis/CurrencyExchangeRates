using CurrencyExchangeRates.Application.Common.Jobs.Implementaions;
using CurrencyExchangeRates.EcbGateway.Models;
using CurrencyExchangeRates.EcbGateway.Services.Interfaces;
using CurrencyExchangeRates.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CurrencyExchangeRates.Tests
{
    public class CurrencyRatesUpdateJobTests
    {
        [Fact]
        public async Task ExecuteAsync_UpdatesDatabase()
        {
            // Arrange
            var mockEcbGateway = new Mock<IEcbRatesGateway>();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CurrencyExchangeRates;Trusted_Connection=True;Encrypt=False;")
                .Options;

            var fakeRates = new List<CurrencyRate>
            {
                new CurrencyRate
                {
                    Date = DateTime.UtcNow.Date,
                    Currency = "Euro",
                    Rate = 1
                },
                new CurrencyRate
                {
                    Date = DateTime.UtcNow.Date,
                    Currency = "USD",
                    Rate = 1.1m
                }
            };

            mockEcbGateway.Setup(x => x.GetDailyRatesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeRates);

            using var dbContext = new AppDbContext(options);

            var job = new CurrencyRatesUpdateJob(mockEcbGateway.Object, dbContext);

            // Act
            await job.ExecuteAsync(CancellationToken.None);

            // Assert
            var count = await dbContext.CurrencyRates.CountAsync();
            Assert.Equal(2, count);

            var usdRate = await dbContext.CurrencyRates.FirstOrDefaultAsync(x => x.CurrencyCode == "USD");
            Assert.Equal(1.1m, usdRate.Rate);
        }
    }
}
