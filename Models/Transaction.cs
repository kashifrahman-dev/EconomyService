using System.ComponentModel.DataAnnotations;

namespace EconomyService.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string PlayerId { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public int Amount { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}