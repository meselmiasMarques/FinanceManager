using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Requests.Transactions
{
    public class TransactionDeleteRequest : RequestBase
    {
        [Required(ErrorMessage = "O Id da transação é obrigatório.")]
        public int Id { get; set; }
    }
}
