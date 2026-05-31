namespace SpendWise.API.Models
{
    public class MonthlySaving
    {
        public int Id { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public decimal Amount { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
    }
}