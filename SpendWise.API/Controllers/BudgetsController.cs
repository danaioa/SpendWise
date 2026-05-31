using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendWise.API.Data;
using SpendWise.API.DTOs.Budgets;
using System.Security.Claims;

namespace SpendWise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BudgetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyBudgets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return Ok(budgets);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(int id, UpdateBudgetDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            budget.Percentage = dto.Percentage;

            var recurringTotal = await _context.RecurringTransactions
                .Where(r => r.UserId == userId && r.IsActive)
                .SumAsync(r => r.Amount);

            var availableForBudgets = user.MonthlyIncome - recurringTotal;

            budget.Amount = availableForBudgets * dto.Percentage / 100;

            await _context.SaveChangesAsync();

            return Ok(budget);
        }
    }
}