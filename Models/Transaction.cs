using System.Text.Json.Serialization;
using FinanceManager.Models.Enums;

namespace FinanceManager.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ETransactionType Type { get; set; } = ETransactionType.Withdrawal;
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; }
    }
}
