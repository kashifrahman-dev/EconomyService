using System.ComponentModel.DataAnnotations;

namespace EconomyService.Models
{
    public class Wallet
    {
        [Key]
        public string PlayerId { get; set; } = Guid.NewGuid().ToString();

        public int Balance { get; set; } = 0;

        public List<string> Inventory { get; set; } = new();

        public List<string> ClaimedRewards { get; set; } = new();
    }
}