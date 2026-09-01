using FinanceManager.Models;
using FinanceManager.Requests.Categories;
using FinanceManager.Requests.Transactions;

namespace FinanceManager.Repositories
{
    public interface ITransactionRepository
    {
        Task<IQueryable<Transaction>> GetAllAsync(TransactionGetAllRequest request);
        Task<Transaction> GetByIdAsync(int id);
        Task AddAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(Transaction transaction);
        Task CommitAsync();
    }
}
