using EconomyService.Data;
using EconomyService.DTOs;
using EconomyService.Interfaces;
using EconomyService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EconomyService.Controllers
{
    [ApiController]
    [Route("v1/wallets")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public WalletController(
            ApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        // ============================================================
        // CREDIT WALLET
        // ============================================================

        [HttpPost("{playerId}/credit")]
        public async Task<IActionResult> Credit(
            string playerId,
            [FromBody] CreditRequest request)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                return BadRequest(new
                {
                    message = "PlayerId is required."
                });
            }

            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Request body is required."
                });
            }

            if (request.Amount <= 0)
            {
                return BadRequest(new
                {
                    message = "Credit amount must be greater than zero."
                });
            }

            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser == null)
            {
                return Unauthorized(new
                {
                    message = "User not found."
                });
            }

            // Find wallet belonging to the authenticated user.
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == currentUser.Id);

            // Create wallet if user does not have one.
            if (wallet == null)
            {
                wallet = new Wallet
                {
                    PlayerId = playerId,
                    UserId = currentUser.Id,
                    Balance = 0,
                    Inventory = new List<string>(),
                    ClaimedRewards = new List<string>()
                };

                _context.Wallets.Add(wallet);
            }

            wallet.Balance += request.Amount;

            var transaction = new Transaction
            {
                PlayerId = wallet.PlayerId,
                Type = "Credit",
                Amount = request.Amount,
                Description = string.IsNullOrWhiteSpace(request.Reason)
                    ? "Wallet credited"
                    : request.Reason
            };

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            var walletDto = new WalletDto
            {
                PlayerId = wallet.PlayerId,
                Balance = wallet.Balance,
                Inventory = wallet.Inventory,
                ClaimedRewards = wallet.ClaimedRewards
            };

            return Ok(
                ApiResponse<WalletDto>.Ok(
                    walletDto,
                    "Currency credited successfully."
                )
            );
        }

        // ============================================================
        // GET WALLET
        // ============================================================

        [HttpGet("{playerId}")]
        public async Task<IActionResult> GetWallet(string playerId)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser == null)
            {
                return Unauthorized(new
                {
                    message = "User not found."
                });
            }

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w =>
                    w.PlayerId == playerId &&
                    w.UserId == currentUser.Id);

            if (wallet == null)
            {
                return NotFound(new
                {
                    message = "Wallet not found."
                });
            }

            var walletDto = new WalletDto
            {
                PlayerId = wallet.PlayerId,
                Balance = wallet.Balance,
                Inventory = wallet.Inventory,
                ClaimedRewards = wallet.ClaimedRewards
            };

            return Ok(ApiResponse<WalletDto>.Ok(
                walletDto,
                "Wallet fetched successfully."
            ));
        }

        // ============================================================
        // PURCHASE
        // ============================================================

        [HttpPost("{playerId}/purchase")]
        public async Task<IActionResult> Purchase(
            string playerId,
            [FromBody] PurchaseRequest request)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                return BadRequest(new
                {
                    message = "PlayerId is required."
                });
            }

            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Request body is required."
                });
            }

            if (request.Cost <= 0)
            {
                return BadRequest(new
                {
                    message = "Purchase cost must be greater than zero."
                });
            }

            if (string.IsNullOrWhiteSpace(request.ItemName))
            {
                return BadRequest(new
                {
                    message = "Item name is required."
                });
            }

            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser == null)
            {
                return Unauthorized(new
                {
                    message = "User not found."
                });
            }

            // IMPORTANT:
            // Wallet must belong to the currently authenticated user.
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w =>
                    w.UserId == currentUser.Id &&
                    w.PlayerId == playerId);

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

            // Create a new list so EF detects the property change
            // even when a value converter is being used.
            wallet.Inventory = wallet.Inventory
                .Append(request.ItemName.Trim())
                .ToList();

            var transaction = new Transaction
            {
                PlayerId = wallet.PlayerId,
                Type = "Purchase",
                Amount = -request.Cost,
                Description = $"Purchased {request.ItemName.Trim()}"
            };

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            var walletDto = new WalletDto
            {
                PlayerId = wallet.PlayerId,
                Balance = wallet.Balance,
                Inventory = wallet.Inventory,
                ClaimedRewards = wallet.ClaimedRewards
            };

            return Ok(
                ApiResponse<WalletDto>.Ok(
                    walletDto,
                    "Purchase successful."
                )
            );
        }

        // ============================================================
        // CLAIM REWARD
        // ============================================================

        [HttpPost("{playerId}/claim-reward")]
        public async Task<IActionResult> ClaimReward(
            string playerId,
            [FromBody] ClaimRewardRequest request)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                return BadRequest(new
                {
                    message = "PlayerId is required."
                });
            }

            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Request body is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.RewardId))
            {
                return BadRequest(new
                {
                    message = "RewardId is required."
                });
            }

            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser == null)
            {
                return Unauthorized(new
                {
                    message = "User not found."
                });
            }

            // Only the authenticated user's wallet can be modified.
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w =>
                    w.UserId == currentUser.Id &&
                    w.PlayerId == playerId);

            if (wallet == null)
            {
                return NotFound(new
                {
                    message = "Wallet not found."
                });
            }

            var rewardId = request.RewardId.Trim();

            if (wallet.ClaimedRewards.Contains(rewardId))
            {
                return BadRequest(new
                {
                    message = "Reward already claimed."
                });
            }

            // Create a new list so EF detects the modification.
            wallet.ClaimedRewards = wallet.ClaimedRewards
                .Append(rewardId)
                .ToList();

            var transaction = new Transaction
            {
                PlayerId = wallet.PlayerId,
                Type = "Reward",
                Amount = 0,
                Description = $"Claimed reward: {rewardId}"
            };

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            var walletDto = new WalletDto
            {
                PlayerId = wallet.PlayerId,
                Balance = wallet.Balance,
                Inventory = wallet.Inventory,
                ClaimedRewards = wallet.ClaimedRewards
            };

            return Ok(
                ApiResponse<WalletDto>.Ok(
                    walletDto,
                    "Reward claimed successfully."
                )
            );
        }

        // ============================================================
        // GET TRANSACTIONS
        // ============================================================

        [HttpGet("{playerId}/transactions")]
        public async Task<IActionResult> GetTransactions(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                return BadRequest(new
                {
                    message = "PlayerId is required."
                });
            }

            var currentUser = await _currentUserService.GetCurrentUserAsync();

            if (currentUser == null)
            {
                return Unauthorized(new
                {
                    message = "User not found."
                });
            }

            // First verify that this wallet belongs to the current user.
            var wallet = await _context.Wallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w =>
                    w.UserId == currentUser.Id &&
                    w.PlayerId == playerId);

            if (wallet == null)
            {
                return NotFound(new
                {
                    message = "Wallet not found."
                });
            }

            var transactions = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.PlayerId == wallet.PlayerId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransactionDto
                {
                    Type = t.Type,
                    Amount = t.Amount,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(
                ApiResponse<List<TransactionDto>>.Ok(
                    transactions,
                    "Transactions fetched successfully."
                )
            );
        }
    }
}