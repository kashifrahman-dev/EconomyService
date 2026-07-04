namespace EconomyService.DTOs
{
    public class PurchaseRequest
    {
        public string ItemName { get; set; } = string.Empty;
        public int Cost { get; set; }
    }
}