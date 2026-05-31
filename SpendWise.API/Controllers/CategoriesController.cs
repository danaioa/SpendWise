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
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ApplicationDbContext context,
            ILogger<CategoriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var categories = await _context.Categories
                .Where(c => isAdmin || c.UserId == null || c.UserId == userId)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.IsExpense,
                    IsCustom = c.UserId != null
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var category = await _context.Categories
                .Where(c => c.Id == id)
                .Where(c => isAdmin || c.UserId == null || c.UserId == userId)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.IsExpense,
                    IsCustom = c.UserId != null
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest("Category name is required.");
            }

            var newCategory = new Category
            {
                Name = category.Name.Trim(),
                IsExpense = category.IsExpense,
                UserId = userId
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category {CategoryName} was created.", newCategory.Name);

            var response = new
            {
                newCategory.Id,
                newCategory.Name,
                newCategory.IsExpense,
                IsCustom = true
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = newCategory.Id },
                response
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    (isAdmin || c.UserId == userId));

            if (existingCategory == null)
            {
                return NotFound();
            }

            if (!isAdmin && existingCategory.UserId == null)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest("Category name is required.");
            }

            existingCategory.Name = category.Name.Trim();
            existingCategory.IsExpense = category.IsExpense;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Category {CategoryId} was updated.", id);

            return Ok(new
            {
                existingCategory.Id,
                existingCategory.Name,
                existingCategory.IsExpense,
                IsCustom = existingCategory.UserId != null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    (isAdmin || c.UserId == userId));

            if (category == null)
            {
                return NotFound();
            }

            if (!isAdmin && category.UserId == null)
            {
                return Forbid();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category {CategoryId} was deleted.", id);

            return NoContent();
        }
    }
}