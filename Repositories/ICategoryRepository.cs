using FinanceManager.Models;

namespace FinanceManager.Repositories
{
    public interface ICategoryRepository
    {
        Task<IQueryable<Category>> GetAllAsync(int skip, int take);
        Task<Category> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);

        Task CommitAsync();
    }
}
