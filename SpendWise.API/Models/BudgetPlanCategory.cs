namespace SpendWise.API.Models
{
    public class BudgetPlanCategory
    {
        public int BudgetPlanId { get; set; }

        public BudgetPlan BudgetPlan { get; set; } = null!;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public int Priority { get; set; }

        public decimal Percentage { get; set; }
    }
}