using FinanceManager.Models;
using FinanceManager.Requests.Categories;

namespace FinanceManager.Repositories
{
    public interface ICategoryRepository
    {
        /// <summary>Query já filtrada pelo usuário e ordenada. A paginação é responsabilidade do service.</summary>
        IQueryable<Category> GetAll(CategoryGetAllRequest request);

        Task<Category?> GetByIdAsync(int id, int userId);
        Task AddAsync(Category category);
        void Update(Category category);
        void Delete(Category category);
        Task SaveChangesAsync();
    }
}
