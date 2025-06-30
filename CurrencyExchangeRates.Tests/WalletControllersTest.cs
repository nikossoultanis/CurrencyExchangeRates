using CurrencyExchangeRates.Api.Controllers;
using CurrencyExchangeRates.Application.Common.CQRS.Commands.Adjust;
using CurrencyExchangeRates.Application.Common.CQRS.Commands.Create;
using CurrencyExchangeRates.Application.Common.CQRS.Queries;
using CurrencyExchangeRates.Application.Common.Interfaces;
using CurrencyExchangeRates.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Tests
{
    public class WalletControllersTest
    {
        [Fact]
        public async Task Create_ReturnsOk_WhenWalletIsCreated()
        {
            var currency = "USD";
            var mockMediator = new Mock<IMediator>();

            mockMediator
                .Setup(m => m.Send(It.IsAny<CreateWalletCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Wallet("USD"));

            var controller = new WalletController(mockMediator.Object);

            // Act
            var result = await controller.Create("USD");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var walletDto = Assert.IsType<Wallet>(okResult.Value);
            Assert.Equal("USD", walletDto.Currency);
            Assert.Equal(0m, walletDto.Balance);
        }

        [Fact]
        public async Task GetBalance_ReturnsOkResult_WithBalance()
        {
            // Arrange
            var walletId = 123;
            var currency = "USD";
            var expectedBalance = 150.75m;

            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send(It.Is<GetWalletBalanceQuery>(q => q.Id == walletId && q.Currency == currency), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedBalance);

            var controller = new WalletController(mockMediator.Object);

            // Act
            var result = await controller.GetBalance(walletId, currency);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBalance = Assert.IsType<decimal>(okResult.Value);
            Assert.Equal(expectedBalance, returnedBalance);
        }
        [Fact]
        public async Task GetBalance_ReturnsOkResult_WhenCurrencyIsNull()
        {
            // Arrange
            var walletId = 456;
            string? currency = null;
            var expectedBalance = 99.99m;

            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send(It.Is<GetWalletBalanceQuery>(q => q.Id == walletId && q.Currency == currency), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedBalance);

            var controller = new WalletController(mockMediator.Object);

            // Act
            var result = await controller.GetBalance(walletId, currency);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBalance = Assert.IsType<decimal>(okResult.Value);
            Assert.Equal(expectedBalance, returnedBalance);
        }

        [Fact]
        public async Task AdjustBalance_ReturnsOkResult_WhenAdjustmentIsSuccessful()
        {
            // Arrange
            var walletId = 10;
            var amount = 50.00m;
            var currency = "USD";
            var strategy = "AddFundsStrategy";

            var mockMediator = new Mock<IMediator>();
            mockMediator
                .Setup(m => m.Send(It.Is<AdjustWalletBalanceCommand>(cmd =>
                    cmd.Id == walletId &&
                    cmd.Amount == amount &&
                    cmd.Currency == currency &&
                    cmd.Strategy == strategy), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            var controller = new WalletController(mockMediator.Object);

            // Act
            var result = await controller.AdjustBalance(walletId, amount, currency, strategy);

            // Assert
            Assert.IsType<OkResult>(result);
            mockMediator.Verify(m => m.Send(It.IsAny<AdjustWalletBalanceCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
