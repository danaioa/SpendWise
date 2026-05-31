namespace SpendWise.API.Models
{
    public class BudgetPlan
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public ICollection<BudgetPlanCategory> BudgetPlanCategories { get; set; }
            = new List<BudgetPlanCategory>();
    }
}