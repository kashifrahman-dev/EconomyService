namespace EconomyService.DTOs
{
    public class PurchaseResponseDto
    {
        public string PlayerId { get; set; } = string.Empty;

        public int Balance { get; set; }

        public List<string> Inventory { get; set; } = new();
    }
}