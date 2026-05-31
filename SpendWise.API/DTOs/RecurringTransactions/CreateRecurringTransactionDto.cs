using System.ComponentModel.DataAnnotations;

namespace SpendWise.API.DTOs.RecurringTransactions
{
    public class CreateRecurringTransactionDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 1000000)]
        public decimal Amount { get; set; }

        [Range(1, 31)]
        public int DayOfMonth { get; set; }

        public int? CategoryId { get; set; }
    }
}