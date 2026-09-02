using FinanceManager.Services;

namespace FinanceManager.Extensions.Endpoints
{
    public class DashboardEndPoints : IEndPoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/dashboard").WithTags("Dashboard").RequireAuthorization();

            group.MapGet("/summary", async (IDashboardService service, int monthsBack = 6) =>
            {
                var result = await service.GetSummaryAsync(monthsBack);
                return result.ToHttpResult();
            })
            .WithDescription("Resumo financeiro: totais, saldo, quebra por categoria e série mensal.");
        }
    }
}
