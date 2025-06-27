using CurrencyExchangeRates.Application.Domain.Entities.WalletLogic;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchangeRates.Api.Controllers
{
    [ApiController]
    [Route("api/wallets")]
    public class WalletController : Controller
    {
        private readonly WalletService _walletService;

        public WalletController(WalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string currency)
        {
            var wallet = await _walletService.CreateWalletAsync(currency);
            return Ok(wallet);
        }

        [HttpGet("{id}/balance")]
        public async Task<IActionResult> GetBalance(long id, [FromQuery] string? currency = null)
        {
            var balance = await _walletService.GetBalanceAsync(id, currency);
            return Ok(balance);
        }

        [HttpPost("{id}/adjust")]
        public async Task<IActionResult> AdjustBalance(long id, [FromQuery] decimal amount, [FromQuery] string currency, [FromQuery] string strategy)
        {
            await _walletService.AdjustBalanceAsync(id, amount, currency, strategy);
            return Ok();
        }
    }
}
