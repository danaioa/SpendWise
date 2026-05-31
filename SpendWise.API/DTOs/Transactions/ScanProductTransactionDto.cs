using System.ComponentModel.DataAnnotations;

namespace SpendWise.API.DTOs.Transactions
{
    public class ScanProductTransactionDto
    {
        [Required]
        public string ProductCode { get; set; } = string.Empty;

        [Range(0.01, 999999)]
        public decimal Quantity { get; set; }
    }
}