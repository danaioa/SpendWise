using SpendWise.API.Models;

namespace SpendWise.API.Services.Interfaces
{
    public interface IBudgetPlanService
    {
        Task<List<BudgetPlan>> GetAllAsync(string userId);
        Task<bool> ApplyPlanAsync(int planId, string userId);
    }
}