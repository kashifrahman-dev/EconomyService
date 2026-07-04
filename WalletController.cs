using EconomyService.Data;
using EconomyService.DTOs;
using EconomyService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            _context.Transactions.Add(new Transaction
            {
                PlayerId = wallet.PlayerId,
                Type = "Credit",
                Amount = request.Amount,
                Description = request.Reason
            });

            await _context.SaveChangesAsync();
            var walletDto = new WalletDto
            {
                PlayerId = wallet.PlayerId,
                Balance = wallet.Balance,
                Inventory = wallet.Inventory,
                ClaimedRewards = wallet.ClaimedRewards
            };

            return Ok(ApiResponse<WalletDto>.Ok(
                 walletDto,
                 "Currency credited successfully."
            ));
        }

        [HttpGet("{playerId}")]
        public async Task<IActionResult> GetWallet(string playerId)
        {
            var wallet = await _context.Wallets.FindAsync(playerId);

            if (wallet == null)
            {
                return NotFound(new
                {
                    message = "Wallet not found."
                });
            }

            return Ok(new
            {
                playerId = wallet.PlayerId,
                balance = wallet.Balance,
                inventory = wallet.Inventory,
                claimedRewards = wallet.ClaimedRewards
            });
        }

        [HttpPost("{playerId}/purchase")]
        public async Task<IActionResult> Purchase(string playerId, PurchaseRequest request)
        {
            var wallet = await _context.Wallets.FindAsync(playerId);

            if (wallet == null)
            {
                return NotFound(new
                {
                    message = "Wallet not found."
                });
            }

            if (wallet.Balance < request.Cost)
            {
                return BadRequest(new
                {
                    message = "Insufficient balance."
                });
            }

            wallet.Balance -= request.Cost;
            wallet.Inventory.Add(request.ItemName);
            _context.Transactions.Add(new Transaction
            {
                PlayerId = wallet.PlayerId,
                Type = "Purchase",
                Amount = -request.Cost,
                Description = $"Purchased {request.ItemName}"
            });
            // wallet.Inventory = request.ItemName;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                playerId = wallet.PlayerId,
                balance = wallet.Balance,
                inventory = wallet.Inventory,
                message = "Purchase successful."
            });
        }

        [HttpPost("{playerId}/claim-reward")]
        public async Task<IActionResult> ClaimReward(string playerId, ClaimRewardRequest request)
        {
            var wallet = await _context.Wallets.FindAsync(playerId);

            if (wallet == null)
            {
                return NotFound(new
                {
                    message = "Wallet not found."
                });
            }

            if (wallet.ClaimedRewards.Contains(request.RewardId))
            {
                return BadRequest(new
                {
                    message = "Reward already claimed."
                });
            }

            wallet.ClaimedRewards.Add(request.RewardId);
            _context.Transactions.Add(new Transaction
            {
                PlayerId = wallet.PlayerId,
                Type = "Reward",
                Amount = 0,
                Description = $"Claimed reward: {request.RewardId}"
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                playerId = wallet.PlayerId,
                claimedRewards = wallet.ClaimedRewards,
                message = "Reward claimed successfully."
            });
        }

        [HttpGet("{playerId}/transactions")]
        public async Task<IActionResult> GetTransactions(string playerId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.PlayerId == playerId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransactionDto
                {
                    Type = t.Type,
                    Amount = t.Amount,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<TransactionDto>>.Ok(
                transactions,
                "Transactions fetched successfully."));
        }
    }
}