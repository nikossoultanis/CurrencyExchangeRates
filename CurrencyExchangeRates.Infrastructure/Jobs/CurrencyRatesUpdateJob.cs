using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Application.Common.Jobs.Interfaces;
using CurrencyExchangeRates.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeRates.Application.Common.Jobs.Implementaions
{

    public class CurrencyRatesUpdateJob : ICurrencyRatesUpdateJob
    {
        private readonly IGatewayService _ratesGateway;
        private readonly AppDbContext _dbContext;
        private readonly ICurrencyRateService _currencyRateService;

        public CurrencyRatesUpdateJob(
            IGatewayService rateGateway,
            AppDbContext dbContext,
            ICurrencyRateService currencyRateService)
        {
            _ratesGateway = rateGateway;
            _dbContext = dbContext;
            _currencyRateService = currencyRateService;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var rates = await _ratesGateway.GetDailyRatesAsync(cancellationToken);

            var rows = rates.Select(r =>
                $"('{r.CurrencyCode}', {r.Rate.ToString(System.Globalization.CultureInfo.InvariantCulture)}, '{r.Date:yyyy-MM-dd}')"
            );

            var valuesSql = string.Join(",", rows);

            var sql = $@"
                        MERGE INTO CurrencyRates AS Target
                        USING
                        (
                            VALUES {valuesSql}
                        ) AS Source (CurrencyCode, Rate, Date)
                        ON Target.CurrencyCode = Source.CurrencyCode AND Target.Date = Source.Date
                        WHEN MATCHED THEN
                            UPDATE SET Rate = Source.Rate
                        WHEN NOT MATCHED THEN
                            INSERT (CurrencyCode, Rate, Date)
                            VALUES (Source.CurrencyCode, Source.Rate, Source.Date);";

            var completed = await _dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
            _currencyRateService.SetRatesAsync(rates);
        }
    }
}
