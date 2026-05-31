namespace SpendWise.API.Models
{
    public class Budget
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Percentage { get; set; }

        public decimal Amount { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
    }
}