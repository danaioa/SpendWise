namespace SpendWise.API.Models
{
    public class RecurringTransaction
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public int DayOfMonth { get; set; }

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}