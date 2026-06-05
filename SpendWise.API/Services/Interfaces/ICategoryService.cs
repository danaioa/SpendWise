using SpendWise.API.Models;

namespace SpendWise.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<object>> GetAllAsync(string userId, bool isAdmin);
        Task<object?> GetByIdAsync(int id, string userId, bool isAdmin);
        Task<object?> CreateAsync(Category category, string userId);
        Task<object?> UpdateAsync(int id, Category category, string userId, bool isAdmin);
        Task<bool?> DeleteAsync(int id, string userId, bool isAdmin);
    }
}