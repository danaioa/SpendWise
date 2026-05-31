namespace SpendWise.API.DTOs.Budgets
{
    public class CreateBudgetDto
    {
        public string Name { get; set; } = string.Empty;

        public decimal Percentage { get; set; }

        public decimal Amount { get; set; }
    }
}