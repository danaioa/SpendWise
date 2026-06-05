using SpendWise.API.Models;

namespace SpendWise.API.Services.Interfaces
{
    public interface IMonthlySavingsService
    {
        Task<List<MonthlySaving>> GetMySavingsAsync(string userId);
        Task<MonthlySaving?> CloseMonthAsync(string userId);
    }
}