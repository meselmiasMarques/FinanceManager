using FinanceManager.Responses;
using FinanceManager.Responses.Dashboard;

namespace FinanceManager.Services
{
    public interface IDashboardService
    {
        Task<Response<DashboardSummaryResponse>> GetSummaryAsync(int monthsBack = 6);
    }
}
