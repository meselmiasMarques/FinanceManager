using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Requests.Transactions
{
    public class TransactionDeleteRequest : RequestBase
    {
        [Required(ErrorMessage = "Transaction Id is required.")]
        public int Id { get; set; }
    }
}
