using EconomyService.Data;
using EconomyService.DTOs;
using EconomyService.Models;
using Microsoft.AspNetCore.Mvc;

namespace EconomyService.Controllers
{
    [ApiController]
    [Route("v1/wallets")]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WalletController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("{playerId}/credit")]
        public async Task<IActionResult> Credit(string playerId, CreditRequest request)
        {
            var wallet = await _context.Wallets.FindAsync(playerId);

            if (wallet == null)
            {
                wallet = new Wallet
                {
                    PlayerId = playerId,
                    Balance = 0
                };

                _context.Wallets.Add(wallet);
            }

            wallet.Balance += request.Amount;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                playerId = wallet.PlayerId,
                balance = wallet.Balance,
                message = "Currency credited successfully."
            });
        }
    }
}