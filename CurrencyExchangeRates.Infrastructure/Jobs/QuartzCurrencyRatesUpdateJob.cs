using CurrencyExchangeRates.Application.Common.Jobs.Interfaces;
using Quartz;

namespace CurrencyExchangeRates.Infrastructure.Jobs
{

    public class QuartzCurrencyRatesUpdateJob : IJob
    {
        private readonly ICurrencyRatesUpdateJob _job;
        public QuartzCurrencyRatesUpdateJob(ICurrencyRatesUpdateJob job)
        {
            _job = job;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _job.ExecuteAsync(context.CancellationToken);
        }
    }

}
