using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Requests.Transactions
{
    public class TransactionGetByIdRequest : RequestBase
    {
        [Required(ErrorMessage = "Transaction Id is required.")]
        public int Id { get; set; }
    }
}
