﻿using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Application.Common.Services;
using Microsoft.Extensions.Logging;
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
            var providerName = context.JobDetail.JobDataMap.GetString("ECBProvider") ?? "ECB";
            await _job.ExecuteAsync(context.CancellationToken, providerName);
        }
    }

}
