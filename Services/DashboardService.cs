using FinanceManager.Extensions;
using FinanceManager.Models.Enums;
using FinanceManager.Repositories;
using FinanceManager.Requests.Categories;
using FinanceManager.Responses;
using FinanceManager.Responses.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Services
{
    public class DashboardService(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        IUserContext user,
        ILogger<DashboardService> logger) : IDashboardService
    {
        public async Task<Response<DashboardSummaryResponse>> GetSummaryAsync(int monthsBack = 6)
        {
            try
            {
                monthsBack = Math.Clamp(monthsBack, 1, 24);

                var rows = await transactionRepository
                    .QueryByUser(user.UserId)
                    .Select(t => new { t.Id, t.Title, t.Amount, t.Type, t.CreatedAt, t.CategoryId })
                    .ToListAsync();

                var categoryNames = await categoryRepository
                    .GetAll(new CategoryGetAllRequest { UserId = user.UserId })
                    .ToDictionaryAsync(c => c.Id, c => c.Name);

                var summary = new DashboardSummaryResponse
                {
                    TransactionCount = rows.Count,
                    TotalIncome = rows.Where(r => r.Type == ETransactionType.Deposit).Sum(r => r.Amount),
                    TotalExpense = rows.Where(r => r.Type == ETransactionType.Withdrawal).Sum(r => r.Amount),
                };
                summary.Balance = summary.TotalIncome - summary.TotalExpense;

                summary.ByCategory = rows
                    .GroupBy(r => r.CategoryId)
                    .Select(g => new CategoryBreakdownItem
                    {
                        CategoryId = g.Key,
                        CategoryName = categoryNames.GetValueOrDefault(g.Key, "(sem categoria)"),
                        Income = g.Where(x => x.Type == ETransactionType.Deposit).Sum(x => x.Amount),
                        Expense = g.Where(x => x.Type == ETransactionType.Withdrawal).Sum(x => x.Amount),
                        TransactionCount = g.Count(),
                    })
                    .Select(c => { c.Balance = c.Income - c.Expense; return c; })
                    .OrderByDescending(c => c.Expense)
                    .ToList();

                var firstMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddMonths(-(monthsBack - 1));

                summary.Monthly = Enumerable.Range(0, monthsBack)
                    .Select(offset => firstMonth.AddMonths(offset))
                    .Select(month =>
                    {
                        var monthRows = rows.Where(r =>
                            r.CreatedAt.Year == month.Year && r.CreatedAt.Month == month.Month).ToList();
                        var income = monthRows.Where(x => x.Type == ETransactionType.Deposit).Sum(x => x.Amount);
                        var expense = monthRows.Where(x => x.Type == ETransactionType.Withdrawal).Sum(x => x.Amount);
                        return new MonthlySeriesItem
                        {
                            Year = month.Year,
                            Month = month.Month,
                            Income = income,
                            Expense = expense,
                            Balance = income - expense,
                        };
                    })
                    .ToList();

                summary.RecentTransactions = rows
                    .OrderByDescending(r => r.CreatedAt)
                    .ThenByDescending(r => r.Id)
                    .Take(5)
                    .Select(r => new RecentTransactionItem
                    {
                        Id = r.Id,
                        Title = r.Title,
                        Type = r.Type,
                        Amount = r.Amount,
                        CreatedAt = r.CreatedAt,
                        CategoryId = r.CategoryId,
                        CategoryName = categoryNames.GetValueOrDefault(r.CategoryId, "(sem categoria)"),
                    })
                    .ToList();

                return new Response<DashboardSummaryResponse>(summary, 200, "Resumo gerado com sucesso.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao gerar o resumo do dashboard");
                return new Response<DashboardSummaryResponse>(null, 500, "Ocorreu um erro ao gerar o resumo financeiro.");
            }
        }
    }
}
