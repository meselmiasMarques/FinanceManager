using FinanceManager.Models;
using FinanceManager.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Requests.Transactions
{
    public class TransactionCreateRequest : RequestBase
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required. Withdrawal = 1, Deposit = 2")]
        public ETransactionType Type { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; } 

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }
    }
}
