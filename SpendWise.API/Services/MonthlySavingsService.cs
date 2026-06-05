using Microsoft.EntityFrameworkCore;
using SpendWise.API.Data;
using SpendWise.API.Models;
using SpendWise.API.Services.Interfaces;

namespace SpendWise.API.Services
{
    public class MonthlySavingsService : IMonthlySavingsService
    {
        private readonly ApplicationDbContext _context;

        public MonthlySavingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MonthlySaving>> GetMySavingsAsync(string userId)
        {
            await AutoClosePreviousMonth(userId);

            return await _context.MonthlySavings
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.Month)
                .ToListAsync();
        }

        public async Task<MonthlySaving?> CloseMonthAsync(string userId)
        {
            return await SaveMonthIfNotExists(
                userId,
                DateTime.Today.Year,
                DateTime.Today.Month
            );
        }

        private async Task AutoClosePreviousMonth(string userId)
        {
            var previousMonthDate = DateTime.Today.AddMonths(-1);

            await SaveMonthIfNotExists(
                userId,
                previousMonthDate.Year,
                previousMonthDate.Month
            );
        }

        private async Task<MonthlySaving?> SaveMonthIfNotExists(string userId, int year, int month)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return null;

            var alreadySaved = await _context.MonthlySavings.AnyAsync(s =>
                s.UserId == userId &&
                s.Month == month &&
                s.Year == year);

            if (alreadySaved)
                return null;

            var firstDayOfMonth = new DateTime(year, month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            var transactionExpenses = await _context.Transactions
                .Include(t => t.Category)
                .Where(t =>
                    t.UserId == userId &&
                    t.Category != null &&
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
                Month = month,
                Year = year,
                Amount = savedAmount,
                UserId = userId
            };

            _context.MonthlySavings.Add(saving);
            await _context.SaveChangesAsync();

            return saving;
        }
    }
}