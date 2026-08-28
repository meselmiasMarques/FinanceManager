using FinanceManager.Data;
using FinanceManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Repositories
{
    public class CategoryRepository(AppDbContext context) : ICategoryRepository
    {
        public async Task AddAsync(Category category)
            => await context.Categories.AddAsync(category);
        
        public async Task DeleteAsync(Category category) 
            => context.Remove(category);

        public async Task<IQueryable<Category>> GetAllAsync(int skip, int take)
            =>  context.Categories.AsNoTracking().Skip(skip).Take(take);
        
        public async Task<Category> GetByIdAsync(int id)
            => await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id) 
            ?? throw new InvalidOperationException("Category not found");

        public async Task UpdateAsync(Category category)
            => context.Categories.Update(category);
       
        public async Task CommitAsync()
            => await context.SaveChangesAsync();
    }
}
