namespace SpendWise.API.DTOs.Budgets
{
    public class BudgetDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Percentage { get; set; }

        public decimal Amount { get; set; }
    }
}