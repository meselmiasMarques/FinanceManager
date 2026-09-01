using FinanceManager.Models.Enums;

namespace FinanceManager.Responses.Dashboard
{
    public class DashboardSummaryResponse
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public int TransactionCount { get; set; }

        public List<CategoryBreakdownItem> ByCategory { get; set; } = [];
        public List<MonthlySeriesItem> Monthly { get; set; } = [];
        public List<RecentTransactionItem> RecentTransactions { get; set; } = [];
    }

    public class CategoryBreakdownItem
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Balance { get; set; }
        public int TransactionCount { get; set; }
    }

    public class MonthlySeriesItem
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Balance { get; set; }
    }

    public class RecentTransactionItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public ETransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
