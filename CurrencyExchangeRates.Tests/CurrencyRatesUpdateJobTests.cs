using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Application.Common.Jobs.Implementaions;
using CurrencyExchangeRates.Domain.Entities;
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
            var mockEcbGateway = new Mock<IGatewayService>();
            var mockCurrencyRateService = new Mock<ICurrencyRateService>();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CurrencyExchangeRates;Trusted_Connection=True;Encrypt=False;")
                .Options;


            mockEcbGateway.Setup(x => x.GetDailyRatesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CurrencyRate>
                {
                    new CurrencyRate { CurrencyCode = "USD", Rate = 1.17m, Date = DateTime.Parse("2025-06-27") },
                    new CurrencyRate { CurrencyCode = "EUR", Rate = 1.0m,  Date = DateTime.Parse("2025-06-27") },
                });

            using var dbContext = new AppDbContext(options);

            var job = new CurrencyRatesUpdateJob(mockEcbGateway.Object, dbContext, mockCurrencyRateService.Object);

            // Act
            await job.ExecuteAsync(CancellationToken.None);

            // Assert
            var count = await dbContext.CurrencyRates.CountAsync();
            Assert.Equal(32, count);

            var usdRate = await dbContext.CurrencyRates.FirstOrDefaultAsync(x => x.CurrencyCode == "USD");
            Assert.Equal(1.17m, usdRate.Rate);
        }
    }
}
