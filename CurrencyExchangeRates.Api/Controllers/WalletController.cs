using CurrencyExchangeRates.Application.Common.CQRS.Commands.Adjust;
using CurrencyExchangeRates.Application.Common.CQRS.Commands.Create;
using CurrencyExchangeRates.Application.Common.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchangeRates.Api.Controllers
{
    [ApiController]
    [Route("api/wallets")]
    public class WalletController : Controller
    {
        private readonly IMediator _mediator;

        public WalletController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string currency)
        {
            try
            {
                var command = new CreateWalletCommand { Currency = currency };
                var wallet = await _mediator.Send(command);
                return Ok(wallet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }

        [HttpGet("{id}/balance")]
        public async Task<IActionResult> GetBalance(long id, [FromQuery] string? currency = null)
        {
            var result = await _mediator.Send(new GetWalletBalanceQuery
            {
                Id = id,
                Currency = currency
            });

            return Ok(result);
        }

        [HttpPost("{id}/adjust")]
        public async Task<IActionResult> AdjustBalance(long id, [FromQuery] decimal amount, [FromQuery] string currency, [FromQuery] string strategy)
        {
            var command = new AdjustWalletBalanceCommand
            {
                Id = id,
                Amount = amount,
                Currency = currency,
                Strategy = strategy
            };

            await _mediator.Send(command);
            return Ok();
        }
    }
}
