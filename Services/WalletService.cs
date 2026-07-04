using EconomyService.Data;
using EconomyService.DTOs;

namespace EconomyService.Services
{
    public class WalletService : IWalletService
    {
        private readonly ApplicationDbContext _context;

        public WalletService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<ApiResponse<WalletDto>> ClaimRewardAsync(string playerId, ClaimRewardRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<WalletDto>> CreditAsync(string playerId, CreditRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<TransactionDto>>> GetTransactionsAsync(string playerId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<WalletDto>> GetWalletAsync(string playerId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<WalletDto>> PurchaseAsync(string playerId, PurchaseRequest request)
        {
            throw new NotImplementedException();
        }
    }
}