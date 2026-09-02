using FinanceManager.Web.Models;

namespace FinanceManager.Web.Services;

public class DashboardApiClient(HttpClient http) : ApiClientBase(http)
{
    public Task<ApiResponse<DashboardSummary>> GetSummaryAsync(int monthsBack = 6, CancellationToken ct = default)
        => SendAsync<DashboardSummary>(Json(HttpMethod.Get, $"dashboard/summary?monthsBack={monthsBack}"), ct);
}
