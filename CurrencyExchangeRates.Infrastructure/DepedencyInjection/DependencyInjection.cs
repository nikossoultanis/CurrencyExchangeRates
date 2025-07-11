﻿using CurrencyExchangeRates.Application.Common.CQRS.Queries;
using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Application.Common.Jobs.Implementaions;
using CurrencyExchangeRates.Application.Common.PipelineValidator;
using CurrencyExchangeRates.Application.Common.Services;
using CurrencyExchangeRates.Application.WalletLogic;
using CurrencyExchangeRates.Domain.Repositories;
using CurrencyExchangeRates.Infrastructure.Configuration;
using CurrencyExchangeRates.Infrastructure.Gateways;
using CurrencyExchangeRates.Infrastructure.Jobs;
using CurrencyExchangeRates.Infrastructure.Persistence;
using CurrencyExchangeRates.Infrastructure.Repository;
using CurrencyExchangeRates.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace CurrencyExchangeRates.Infrastructure.DepedencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddHttpClient<EcbGateway>();
            services.AddHttpClient<OpenExchangeGateway>();
            services.AddScoped<ICurrencyGatewayFactory, CurrencyGatewayFactory>();
            services.AddScoped<IEnumerable<ICurrencyGateway>>(sp =>
            {
                return new List<ICurrencyGateway>
                {
                    sp.GetRequiredService<EcbGateway>(),
                    sp.GetRequiredService<OpenExchangeGateway>()
                };
            });
            services.AddScoped<Application.Common.Services.CurrencyRateService>();

            services.AddScoped<ICurrencyRatesUpdateJob, CurrencyRatesUpdateJob>();

            services.AddQuartz(q =>
            {
                // Define the job with identity and add the ECBProvider key
                var jobKey = new JobKey("CurrencyRatesUpdateJob", "CurrencyJobs");

                q.AddJob<QuartzCurrencyRatesUpdateJob>(opts =>
                    opts.WithIdentity(jobKey)
                        .UsingJobData("ECBProvider", "ECB")
                );

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("CurrencyRatesUpdateTrigger", "CurrencyTriggers")
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever())
                );
            });


            services.AddQuartzHostedService();
            services.AddScoped<ICurrencyRateRepository, CurrencyRateRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IFundsAdjustmentStrategy, AddFundsStrategy>();
            services.AddScoped<IFundsAdjustmentStrategy, SubtractFundsStrategy>();
            services.AddScoped<IFundsAdjustmentStrategy, ForceSubtractFundsStrategy>();
            services.AddScoped<WalletService>(); 
            services.AddMemoryCache();
            services.AddScoped<ICurrencyRateService, Services.CurrencyRateService>();
            services.AddScoped<IWalletService, WalletService>();

            // MediatR
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(GetWalletBalanceQuery).Assembly));
            // FluentValidation
            services.AddValidatorsFromAssembly(typeof(GetWalletBalanceQueryValidator).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EndpointInputValidation<,>));

            return services;
        }
    }
}
