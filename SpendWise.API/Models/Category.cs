namespace SpendWise.API.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsExpense { get; set; }

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public ICollection<BudgetPlanCategory> BudgetPlanCategories { get; set; }
            = new List<BudgetPlanCategory>();

        public ICollection<Transaction> Transactions { get; set; }
            = new List<Transaction>();
    }
}