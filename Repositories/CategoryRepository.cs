using FinanceManager.Data;
using FinanceManager.Models;
using FinanceManager.Requests.Categories;
using FinanceManager.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FinanceManager.Repositories
{
    public class CategoryRepository(AppDbContext context) : ICategoryRepository
    {
        public async Task AddAsync(Category category)
            => await context.Categories.AddAsync(category);
        
        public async Task DeleteAsync(Category category) 
            => context.Remove(category);

        public async Task<IQueryable<Category>> GetAllAsync(CategoryGetAllRequest request)
        {
            var query = context
                 .Categories
                 .AsNoTracking()
                 .Skip(request.PageNumber)
                 .Take(request.PageSize)
                 .Where(c => c.UserId == request.UserId);


            return query;


        }
        
        public async Task<Category> GetByIdAsync(int id)
            => await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id) 
            ?? throw new InvalidOperationException("Category not found");

        public async Task UpdateAsync(Category category)
            => context.Categories.Update(category);
       
        public async Task CommitAsync()
            => await context.SaveChangesAsync();
    }

   
}
