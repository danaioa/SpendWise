using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendWise.API.Data;
using SpendWise.API.DTOs.Dashboard;
using System.Security.Claims;

namespace SpendWise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            var transactionExpenses = await _context.Transactions
                .Include(t => t.Category)
                .Where(t =>
                    t.UserId == userId &&
                    t.Category.IsExpense &&
                    t.Date >= firstDayOfMonth &&
                    t.Date < firstDayOfNextMonth)
                .SumAsync(t => t.Amount);

            var recurringExpenses = await _context.RecurringTransactions
                .Where(r => r.UserId == userId && r.IsActive)
                .SumAsync(r => r.Amount);

            var totalExpenses = transactionExpenses + recurringExpenses;

            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId)
                .ToListAsync();

            var categoryExpenses = new List<CategoryExpenseDto>();

            foreach (var budget in budgets)
            {
                var spent = await _context.Transactions
                    .Include(t => t.Category)
                    .Where(t =>
                        t.UserId == userId &&
                        t.Category.IsExpense &&
                        t.Category.Name == budget.Name &&
                        t.Date >= firstDayOfMonth &&
                        t.Date < firstDayOfNextMonth)
                    .SumAsync(t => (decimal?)t.Amount) ?? 0;

                categoryExpenses.Add(new CategoryExpenseDto
                {
                    Name = budget.Name,
                    BudgetAmount = (user.MonthlyIncome - recurringExpenses) * budget.Percentage / 100,
                    SpentAmount = spent
                });
            }

            return Ok(new DashboardSummaryDto
            {
                MonthlyIncome = user.MonthlyIncome,
                TotalExpenses = totalExpenses,
                RemainingBalance = user.MonthlyIncome - totalExpenses,

                Budgets = budgets.Select(b => new BudgetSummaryDto
                {
                    Name = b.Name,
                    Percentage = b.Percentage,
                    Amount = b.Amount
                }).ToList(),

                CategoryExpenses = categoryExpenses
            });
        }
    }
}