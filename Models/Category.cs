using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManager.Models
{
    public class Category
    {
        public int Id { get; set; }
  
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int UserId { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    };
}
