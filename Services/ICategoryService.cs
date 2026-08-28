using FinanceManager.Models;
using FinanceManager.Requests.Categories;
using FinanceManager.Responses;

namespace FinanceManager.Services
{
    public interface ICategoryService
    {
        Task<Response<List<Category>>> GetAllAsync(int skip, int take);
        Task<Response<Category>> GetByIdAsync(CategoryGetByIdRequest request);
        Task<Response<Category>> AddAsync(CategoryCreateRequest request);
        Task<Response<Category>> UpdateAsync(CategoryUpdateRequest request);
        Task<Response<Category>> DeleteAsync(CategoryDeleteRequest request);
    }
}
