using Microsoft.AspNetCore.Identity;

namespace SpendWise.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public decimal MonthlyIncome { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
            = new List<Transaction>();

        public ICollection<Budget> Budgets { get; set; }
            = new List<Budget>();
    }
}