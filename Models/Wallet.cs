using System.ComponentModel.DataAnnotations;

namespace EconomyService.Models
{
    public class Wallet
    {
        [Key]
        public string PlayerId { get; set; } = Guid.NewGuid().ToString();

        public int Balance { get; set; } = 0;

        public string Inventory { get; set; } = "[]";

        public string ClaimedRewards { get; set; } = "[]";
    }
}