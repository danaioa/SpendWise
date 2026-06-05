using Microsoft.EntityFrameworkCore;
using SpendWise.API.Data;
using SpendWise.API.Models;
using SpendWise.API.Services.Interfaces;

namespace SpendWise.API.Services
{
    public class BudgetPlanService : IBudgetPlanService
    {
        private readonly ApplicationDbContext _context;

        public BudgetPlanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BudgetPlan>> GetAllAsync(string userId)
        {
            return await _context.BudgetPlans
                .Include(p => p.BudgetPlanCategories)
                .ThenInclude(pc => pc.Category)
                .Where(p => p.UserId == null || p.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> ApplyPlanAsync(int planId, string userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return false;

            var plan = await _context.BudgetPlans
                .Include(p => p.BudgetPlanCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == planId);

            if (plan == null)
                return false;

            var today = DateTime.Today;

            var oldBudgets = await _context.Budgets
                .Where(b =>
                    b.UserId == userId &&
                    b.Month == today.Month &&
                    b.Year == today.Year)
                .ToListAsync();

            _context.Budgets.RemoveRange(oldBudgets);

            foreach (var item in plan.BudgetPlanCategories)
            {
                var amount = user.MonthlyIncome * item.Percentage / 100;

                var budget = new Budget
                {
                    Name = item.Category.Name,
                    Percentage = item.Percentage,
                    Amount = amount,
                    Month = today.Month,
                    Year = today.Year,
                    UserId = userId
                };

                _context.Budgets.Add(budget);
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}