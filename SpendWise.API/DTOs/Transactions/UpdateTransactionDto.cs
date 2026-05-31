using System.ComponentModel.DataAnnotations;

namespace SpendWise.API.DTOs.Transactions
{
    public class UpdateTransactionDto
    {
        [Range(0.01, 1000000)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}