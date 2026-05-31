using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendWise.API.Data;
using SpendWise.API.DTOs.RecurringTransactions;
using SpendWise.API.Models;
using System.Security.Claims;

namespace SpendWise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecurringTransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecurringTransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var items = await _context.RecurringTransactions
                .Include(r => r.Category)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRecurringTransactionDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = new RecurringTransaction
            {
                Name = dto.Name,
                Amount = dto.Amount,
                DayOfMonth = dto.DayOfMonth,
                CategoryId = dto.CategoryId,
                UserId = userId!
            };

            _context.RecurringTransactions.Add(item);

            await _context.SaveChangesAsync();

            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            CreateRecurringTransactionDto dto)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var item = await _context.RecurringTransactions
                        .FirstOrDefaultAsync(x =>
                            x.Id == id &&
                            x.UserId == userId);

                    if (item == null)
                        return NotFound();

                    item.Name = dto.Name;
                    item.Amount = dto.Amount;
                    item.DayOfMonth = dto.DayOfMonth;
                    item.CategoryId = dto.CategoryId;

                    await _context.SaveChangesAsync();

                    return NoContent();
                }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.RecurringTransactions.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            _context.RecurringTransactions.Remove(item);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}