using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendWise.API.Data;
using SpendWise.API.Models;
using System.Security.Claims;

namespace SpendWise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MonthlySavingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MonthlySavingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMySavings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var savings = await _context.MonthlySavings
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.Month)
                .ToListAsync();

            return Ok(savings);
        }

        [HttpPost("close-month")]
        public async Task<IActionResult> CloseMonth()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return Unauthorized();

            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            var alreadySaved = await _context.MonthlySavings.AnyAsync(s =>
                s.UserId == userId &&
                s.Month == today.Month &&
                s.Year == today.Year);

            if (alreadySaved)
                return BadRequest("This month is already saved.");

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

            var savedAmount = user.MonthlyIncome - totalExpenses;

            var saving = new MonthlySaving
            {
                Month = today.Month,
                Year = today.Year,
                Amount = savedAmount,
                UserId = userId!
            };

            _context.MonthlySavings.Add(saving);
            await _context.SaveChangesAsync();

            return Ok(saving);
        }
    }
}