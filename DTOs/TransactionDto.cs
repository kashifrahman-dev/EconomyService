namespace EconomyService.DTOs
{
    public class TransactionDto
    {
        public string Type { get; set; } = string.Empty;

        public int Amount { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}