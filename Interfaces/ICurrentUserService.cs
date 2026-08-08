using EconomyService.Models;

namespace EconomyService.Interfaces
{
    public interface ICurrentUserService
    {
        string? GetCurrentUserEmail();

        Task<User?> GetCurrentUserAsync();
    }
}