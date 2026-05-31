namespace SpendWise.API.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public decimal MonthlyIncome { get; set; }

        public decimal TotalExpenses { get; set; }

        public decimal RemainingBalance { get; set; }

        public List<BudgetSummaryDto> Budgets { get; set; } = new();

        public List<CategoryExpenseDto> CategoryExpenses { get; set; } = new();

    }

    public class CategoryExpenseDto
    {
        public string Name { get; set; } = string.Empty;

        public decimal BudgetAmount { get; set; }

        public decimal SpentAmount { get; set; }
    }

    public class BudgetSummaryDto
    {
        public string Name { get; set; } = string.Empty;

        public decimal Percentage { get; set; }

        public decimal Amount { get; set; }
    }
}