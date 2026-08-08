using EconomyService.Data;
using EconomyService.Interfaces;
using EconomyService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EconomyService.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public string? GetCurrentUserEmail()
        {
            return _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.Email)?.Value;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var email = GetCurrentUserEmail();

            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}