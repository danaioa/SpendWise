using SpendWise.API.DTOs.Dashboard;

namespace SpendWise.API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto?> GetSummaryAsync(string userId);
    }
}