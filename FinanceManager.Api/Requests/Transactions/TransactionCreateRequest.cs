using FinanceManager.Models;
using FinanceManager.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Requests.Transactions
{
    public class TransactionCreateRequest : RequestBase
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo é obrigatório (Despesa = 1, Receita = 2).")]
        public ETransactionType Type { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public int CategoryId { get; set; }
    }
}
