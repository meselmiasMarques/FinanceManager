using FinanceManager.Models;
using FinanceManager.Requests.Transactions;

namespace FinanceManager.Repositories
{
    public interface ITransactionRepository
    {
        /// <summary>Query já filtrada pelo usuário e ordenada. A paginação é responsabilidade do service.</summary>
        IQueryable<Transaction> GetAll(TransactionGetAllRequest request);

        Task<Transaction?> GetByIdAsync(int id, int userId);
        Task AddAsync(Transaction transaction);
        void Update(Transaction transaction);
        void Delete(Transaction transaction);
        Task SaveChangesAsync();

        /// <summary>Base para agregações do dashboard (sem ordenação/paginação).</summary>
        IQueryable<Transaction> QueryByUser(int userId);
    }
}
