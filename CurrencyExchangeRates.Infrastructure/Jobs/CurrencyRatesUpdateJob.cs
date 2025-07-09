using Castle.Core.Logging;
using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace CurrencyExchangeRates.Application.Common.Jobs.Implementaions
{

    public class CurrencyRatesUpdateJob : ICurrencyRatesUpdateJob
    {
        private readonly ICurrencyGateway _ratesGateway;
        private readonly ICurrencyGatewayFactory _gatewayFactory;
        private readonly AppDbContext _dbContext;
        private readonly ICurrencyRateService _currencyRateService;
        private readonly ILogger<ICurrencyRatesUpdateJob> _logger;

        public CurrencyRatesUpdateJob(
            ICurrencyGatewayFactory gatewayFactory, 
            ICurrencyGateway rateGateway,
            AppDbContext dbContext,
            ICurrencyRateService currencyRateService,
            ILogger<ICurrencyRatesUpdateJob> logger)
        {
            _gatewayFactory = gatewayFactory;
            _ratesGateway = rateGateway;
            _dbContext = dbContext;
            _currencyRateService = currencyRateService;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken, string providerName = "ECB")
        {
            try
            {
                var gateway = _gatewayFactory.GetGateway(_ratesGateway.GatewayProviderName);
                var rates = await gateway.GetDailyRatesAsync(cancellationToken);

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
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
