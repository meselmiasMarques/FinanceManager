using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Requests.Transactions
{
    public class TransactionGetByIdRequest : RequestBase
    {
        [Required(ErrorMessage = "O Id da transação é obrigatório.")]
        public int Id { get; set; }
    }
}
