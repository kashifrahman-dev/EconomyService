using EconomyService.Models;
using Microsoft.EntityFrameworkCore;

namespace EconomyService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Wallet> Wallets { get; set; }
    }
}