namespace FinanceManager.Web.Models;

public enum ETransactionType
{
    Withdrawal = 1,
    Deposit = 2
}

public static class TransactionTypeExtensions
{
    public static string ToDisplay(this ETransactionType type) => type switch
    {
        ETransactionType.Deposit => "Receita",
        ETransactionType.Withdrawal => "Despesa",
        _ => type.ToString()
    };
}
