using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Web.Models;

public class TransactionModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ETransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Valor com sinal: receitas positivas, despesas negativas (uso local).</summary>
    public decimal SignedAmount => Type == ETransactionType.Deposit ? Amount : -Amount;
}

public class TransactionFormModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(100, ErrorMessage = "O título não pode passar de 100 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o tipo.")]
    [EnumDataType(typeof(ETransactionType), ErrorMessage = "Tipo inválido.")]
    public ETransactionType Type { get; set; } = ETransactionType.Withdrawal;

    [Range(0.01, 999999999, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Amount { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria.")]
    public int CategoryId { get; set; }

    public bool IsNew => Id == 0;

    public static TransactionFormModel FromTransaction(TransactionModel t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Type = t.Type,
        Amount = t.Amount,
        CategoryId = t.CategoryId
    };
}
