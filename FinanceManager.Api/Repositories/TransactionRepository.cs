using FinanceManager.Data;
using FinanceManager.Models;
using FinanceManager.Requests.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Repositories
{
    public class TransactionRepository(AppDbContext context) : ITransactionRepository
    {
        public IQueryable<Transaction> GetAll(TransactionGetAllRequest request)
            => context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == request.UserId)
                .OrderByDescending(t => t.CreatedAt)
                .ThenByDescending(t => t.Id);

        public IQueryable<Transaction> QueryByUser(Guid userId)
            => context.Transactions.AsNoTracking().Where(t => t.UserId == userId);

        public Task<Transaction?> GetByIdAsync(int id, Guid userId)
            => context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        public async Task AddAsync(Transaction transaction)
            => await context.Transactions.AddAsync(transaction);

        public void Update(Transaction transaction)
            => context.Transactions.Update(transaction);

        public void Delete(Transaction transaction)
            => context.Transactions.Remove(transaction);

        public Task SaveChangesAsync()
            => context.SaveChangesAsync();
    }
}
