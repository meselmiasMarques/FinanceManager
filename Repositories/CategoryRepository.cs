using FinanceManager.Data;
using FinanceManager.Models;
using FinanceManager.Requests.Categories;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Repositories
{
    public class CategoryRepository(AppDbContext context) : ICategoryRepository
    {
        public IQueryable<Category> GetAll(CategoryGetAllRequest request)
            => context.Categories
                .AsNoTracking()
                .Where(c => c.UserId == request.UserId)
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Id);

        public Task<Category?> GetByIdAsync(int id, int userId)
            => context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        public async Task AddAsync(Category category)
            => await context.Categories.AddAsync(category);

        public void Update(Category category)
            => context.Categories.Update(category);

        public void Delete(Category category)
            => context.Categories.Remove(category);

        public Task SaveChangesAsync()
            => context.SaveChangesAsync();
    }
}
