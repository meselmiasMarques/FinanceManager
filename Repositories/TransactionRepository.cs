using FinanceManager.Data;
using FinanceManager.Models;
using FinanceManager.Requests.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Repositories
{
    public class TransactionRepository(AppDbContext context) : ITransactionRepository
    {
        public async Task AddAsync(Transaction transaction)
            => await context.Transactions.AddAsync(transaction);
   
        public async Task CommitAsync()
            => await context.SaveChangesAsync();
        

        public async Task DeleteAsync(Transaction transaction)
            => context.Transactions.Remove(transaction);

        public Task<IQueryable<Transaction>> GetAllAsync(TransactionGetAllRequest request)
        {
            var query = context
                .Transactions
                .AsNoTracking()
                .Skip(request.PageNumber)
                .Take(request.PageSize)
                .AsQueryable();

            return Task.FromResult(query);
        }

        public async Task<Transaction> GetByIdAsync(int id)
            => await context.Transactions.FirstOrDefaultAsync(t => t.Id == id);

        public Task UpdateAsync(Transaction transaction)
            => Task.FromResult(context.Transactions.Update(transaction));
    }
}
