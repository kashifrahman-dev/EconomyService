using System.ComponentModel.DataAnnotations;

namespace EconomyService.DTOs
{
    public class CreditRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Amount { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}