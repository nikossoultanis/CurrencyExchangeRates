using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Application.WalletLogic;
using CurrencyExchangeRates.Domain.Entities;
using CurrencyExchangeRates.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CurrencyExchangeRates.Application.Common.Services
{
    public class WalletService : IWalletService
    {
        private readonly ILogger<WalletService> _logger;
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrencyRateRepository _currencyRateRepository;
        private readonly IDictionary<string, IFundsAdjustmentStrategy> _strategies;
        private readonly ICurrencyRateService _currencyRateService;

        public WalletService(
            ILogger<WalletService> logger,
            IWalletRepository walletRepository,
            ICurrencyRateRepository currencyRateRepository,
            IEnumerable<IFundsAdjustmentStrategy> strategies,
            ICurrencyRateService currencyRateService)
        {
            _walletRepository = walletRepository;
            _currencyRateRepository = currencyRateRepository;
            _strategies = new Dictionary<string, IFundsAdjustmentStrategy>(StringComparer.OrdinalIgnoreCase);
            foreach (var strategy in strategies)
            {
                _strategies[strategy.GetType().Name] = strategy;
            }
            _currencyRateService = currencyRateService;
            _logger = logger;
        }

        public async Task<Wallet> CreateWalletAsync(string currency)
        {
            _logger.LogInformation("Creating new wallet in currency", currency);

            var rate = await _currencyRateService.GetLatestRateAsync(currency);

            if (rate == null)
            {
                _logger.LogWarning("Currency {Currency} not supported", currency);
                throw new ArgumentException($"Currency '{currency}' is not supported.");
            }

            var wallet = new Wallet(currency);

            var result = await _walletRepository.CreateAsync(wallet);
            _logger.LogInformation("Wallet {WalletId} created", result.Id);
            return result;
        }

        public async Task<decimal> GetBalanceAsync(long walletId, string? requestedCurrency = null)
        {
            var wallet = await _walletRepository.GetByIdAsync(walletId);
            if (wallet == null)
            {
                _logger.LogWarning("Currency {Currency} not supported", requestedCurrency);
                throw new KeyNotFoundException("Wallet not found."); 
            }

            if (requestedCurrency != null)
            {
                var rate = await _currencyRateService.GetLatestRateAsync(requestedCurrency) ?? throw new ArgumentException($"Currency '{requestedCurrency}' is not supported.");
            }

            if (string.IsNullOrWhiteSpace(requestedCurrency)
                || requestedCurrency.Equals(wallet.Currency, StringComparison.OrdinalIgnoreCase))
            {
                return wallet.Balance;
            }

            var requestedCurrencyNormalized = requestedCurrency.ToUpperInvariant();
            var walletCurrencyNormalized = wallet.Currency.ToUpperInvariant();

            var walletCurrencyRate = await _currencyRateService.GetLatestRateAsync(walletCurrencyNormalized);
            var requestedCurrencyRate = await _currencyRateService.GetLatestRateAsync(requestedCurrencyNormalized);

            if (walletCurrencyRate == null)
            {
                _logger.LogWarning("Exchange rate not found for wallet currency ", requestedCurrency);
                throw new Exception($"Exchange rate not found for wallet currency '{walletCurrencyNormalized}'."); 
            }

            if (requestedCurrencyRate == null)
            {
                _logger.LogWarning("Exchange rate not found for requested currency ", requestedCurrency);
                throw new Exception($"Exchange rate not found for requested currency '{requestedCurrencyNormalized}'."); 
            }

            if (walletCurrencyRate.Rate == 0)
            { 
                _logger.LogWarning("Invalid exchange rate (zero) for wallet currency ", requestedCurrency); 
                throw new Exception($"Invalid exchange rate (zero) for wallet currency '{walletCurrencyNormalized}'."); 
            }

            // Convert balance from wallet currency -> EUR
            var balanceInEur = wallet.Balance / walletCurrencyRate.Rate;

            // Convert from EUR -> requested currency
            var convertedBalance = balanceInEur * requestedCurrencyRate.Rate;

            return convertedBalance;
        }

        public async Task AdjustBalanceAsync(long walletId, decimal amount, string currency, string strategyName)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive.");

            var wallet = await _walletRepository.GetByIdAsync(walletId) ?? throw new KeyNotFoundException("Wallet not found.");

            var strategyKey = strategyName + "Strategy";
            if (!_strategies.TryGetValue(strategyKey, out var strategy))
            {
                _logger.LogWarning($"Strategy '{strategyName}' is not supported.", currency);
                throw new ArgumentException($"Strategy '{strategyName}' is not supported."); 
            }

            // Convert amount to wallet currency
            var walletCurrencyRate = await _currencyRateRepository.GetCurrencyRateAsync(wallet.Currency);
            var amountCurrencyRate = await _currencyRateRepository.GetCurrencyRateAsync(currency);

            if (walletCurrencyRate == null || amountCurrencyRate == null)
            {
                _logger.LogWarning($"Currency rate not available for conversion.");
                throw new Exception("Currency rate not available for conversion."); 
            }

            var amountInEur = amount / amountCurrencyRate.Rate;
            var convertedAmount = amountInEur * walletCurrencyRate.Rate;

            var balance = wallet.Balance;
            strategy.AdjustBalance(convertedAmount, ref balance);

            wallet.Balance = balance;
            await _walletRepository.UpdateAsync(wallet);
        }
    }
    
}
