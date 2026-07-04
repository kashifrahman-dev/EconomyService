using EconomyService.DTOs;

namespace EconomyService.Services
{
    public interface IWalletService
    {
        Task<ApiResponse<WalletDto>> GetWalletAsync(string playerId);

        Task<ApiResponse<WalletDto>> CreditAsync(string playerId, CreditRequest request);

        Task<ApiResponse<WalletDto>> PurchaseAsync(string playerId, PurchaseRequest request);

        Task<ApiResponse<WalletDto>> ClaimRewardAsync(string playerId, ClaimRewardRequest request);

        Task<ApiResponse<List<TransactionDto>>> GetTransactionsAsync(string playerId);
    }
}