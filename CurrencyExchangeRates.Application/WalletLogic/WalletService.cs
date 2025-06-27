using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Application.WalletLogic;
using CurrencyExchangeRates.Domain.CurrencyRateRepository;
using CurrencyExchangeRates.Domain.Entities;
using CurrencyExchangeRates.Domain.WalletRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Application.Domain.Entities.WalletLogic
{
    public class WalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrencyRateRepository _currencyRateRepository;
        private readonly IDictionary<string, IFundsAdjustmentStrategy> _strategies;
        private readonly ICurrencyRateService _currencyRateService;

        public WalletService(
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
        }

        public async Task<Wallet> CreateWalletAsync(string currency)
        {
            var wallet = new Wallet(currency);
            return await _walletRepository.CreateAsync(wallet);
        }

        public async Task<decimal> GetBalanceAsync(long walletId, string? requestedCurrency = null)
        {
            var wallet = await _walletRepository.GetByIdAsync(walletId)
                ?? throw new KeyNotFoundException("Wallet not found.");

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
                throw new Exception($"Exchange rate not found for wallet currency '{walletCurrencyNormalized}'.");

            if (requestedCurrencyRate == null)
                throw new Exception($"Exchange rate not found for requested currency '{requestedCurrencyNormalized}'.");

            if (walletCurrencyRate.Rate == 0)
                throw new Exception($"Invalid exchange rate (zero) for wallet currency '{walletCurrencyNormalized}'.");

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
                throw new ArgumentException($"Strategy '{strategyName}' is not supported.");

            // Convert amount to wallet currency
            var walletCurrencyRate = await _currencyRateRepository.GetCurrencyRateAsync(wallet.Currency);
            var amountCurrencyRate = await _currencyRateRepository.GetCurrencyRateAsync(currency);

            if (walletCurrencyRate == null || amountCurrencyRate == null)
                throw new Exception("Currency rate not available for conversion.");

            // amount in EUR
            var amountInEur = amount / amountCurrencyRate.Rate;
            // converted to wallet currency
            var convertedAmount = amountInEur * walletCurrencyRate.Rate;

            var balance = wallet.Balance;
            strategy.AdjustBalance(convertedAmount, ref balance);

            wallet.Balance = balance;
            await _walletRepository.UpdateAsync(wallet);
        }
    }
    
}
