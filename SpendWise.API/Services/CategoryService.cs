using Microsoft.EntityFrameworkCore;
using SpendWise.API.Data;
using SpendWise.API.Models;
using SpendWise.API.Services.Interfaces;

namespace SpendWise.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<object>> GetAllAsync(string userId, bool isAdmin)
        {
            return await _context.Categories
                .Where(c => isAdmin || c.UserId == null || c.UserId == userId)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.IsExpense,
                    IsCustom = c.UserId != null
                })
                .Cast<object>()
                .ToListAsync();
        }

        public async Task<object?> GetByIdAsync(int id, string userId, bool isAdmin)
        {
            return await _context.Categories
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
        }

        public async Task<object?> CreateAsync(Category category, string userId)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return null;

            var newCategory = new Category
            {
                Name = category.Name.Trim(),
                IsExpense = category.IsExpense,
                UserId = userId
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return new
            {
                newCategory.Id,
                newCategory.Name,
                newCategory.IsExpense,
                IsCustom = true
            };
        }

        public async Task<object?> UpdateAsync(int id, Category category, string userId, bool isAdmin)
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    (isAdmin || c.UserId == userId));

            if (existingCategory == null)
                return null;

            if (!isAdmin && existingCategory.UserId == null)
                return "FORBID";

            if (string.IsNullOrWhiteSpace(category.Name))
                return "BAD_REQUEST";

            existingCategory.Name = category.Name.Trim();
            existingCategory.IsExpense = category.IsExpense;

            await _context.SaveChangesAsync();

            return new
            {
                existingCategory.Id,
                existingCategory.Name,
                existingCategory.IsExpense,
                IsCustom = existingCategory.UserId != null
            };
        }

        public async Task<bool?> DeleteAsync(int id, string userId, bool isAdmin)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    (isAdmin || c.UserId == userId));

            if (category == null)
                return null;

            if (!isAdmin && category.UserId == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}