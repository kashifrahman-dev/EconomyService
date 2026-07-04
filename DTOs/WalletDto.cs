namespace EconomyService.DTOs
{
    public class WalletDto
    {
        public string PlayerId { get; set; } = string.Empty;

        public int Balance { get; set; }

        public List<string> Inventory { get; set; } = new();

        public List<string> ClaimedRewards { get; set; } = new();
    }
}