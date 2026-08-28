using FinanceManager.Models;
using FinanceManager.Repositories;
using FinanceManager.Requests.Categories;
using FinanceManager.Responses;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Services
{
    public class CategoryService(ICategoryRepository repository) : ICategoryService
    {
        public async Task<Response<Category>> AddAsync(CategoryCreateRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
            };

            try
            {
                await repository.AddAsync(category);
                await repository.CommitAsync();

                return new Response<Category>(201, "Categoria criada com sucesso", category);
            }
            catch
            {
                return new Response<Category>(500, "Ocorreu um erro ao criar a categoria.");
            }
        }

        public async Task<Response<Category>> DeleteAsync(CategoryDeleteRequest request)
        {
            try
            {

                var category = await repository.GetByIdAsync(request.Id);

                if (category == null)
                    return new Response<Category>(404, "Categoria não encontrada.");

                await repository.DeleteAsync(category);
                await repository.CommitAsync();
                return new Response<Category>(200, "Categoria deletada com sucesso.", category);
            }
            catch
            {
                return new Response<Category>(500, "Ocorreu um erro ao deletar a categoria.");
            }
        }

        public async Task<Response<List<Category>>> GetAllAsync(int skip, int take)
        {
            try
            {
                var result = await repository.GetAllAsync(skip, take);

                var category = await result.OrderBy(c => c.Name).ToListAsync();

                return new Response<List<Category>>(200, "", category);

            }
            catch
            {
                return new Response<List<Category>>(500, "Ocorreu um erro ao buscar as categorias.");
            }
        }

        public async Task<Response<Category>> GetByIdAsync(CategoryGetByIdRequest request)
        {
            try
            {
                var category = await repository.GetByIdAsync(request.Id);
                if (category == null)
                    return new Response<Category>(404, "Categoria não encontrada.");

                return new Response<Category>(200, "", category);
            }
            catch 
            {
                return new Response<Category>(500, "Ocorreu um erro ao buscar a categoria.");
            }
        }

        public async Task<Response<Category>> UpdateAsync(CategoryUpdateRequest request)
        {
            try
            {
                var category = await repository.GetByIdAsync(request.Id);
                if (category == null)
                    return new Response<Category>(404, "Categoria não encontrada.");

                category.Name = request.Name;
                category.Description = request.Description;

                await repository.UpdateAsync(category);
                await repository.CommitAsync();

                return new Response<Category>(200, "Categoria atualizada com sucesso.", category);
            }
            catch
            {
                return new Response<Category>(500, "Ocorreu um erro ao atualizar a categoria.");
            }
        }
    }
}
