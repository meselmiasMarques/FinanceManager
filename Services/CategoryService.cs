using FinanceManager.Extensions;
using FinanceManager.Models;
using FinanceManager.Repositories;
using FinanceManager.Requests.Categories;
using FinanceManager.Responses;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Services
{
    public class CategoryService(
        ICategoryRepository repository,
        IUserContext user,
        ILogger<CategoryService> logger) : ICategoryService
    {
        public async Task<PagedResponse<List<Category>>> GetAllAsync(CategoryGetAllRequest request)
        {
            try
            {
                request.UserId = user.UserId;

                var query = repository.GetAll(request);
                var totalCount = await query.CountAsync();

                var data = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                return new PagedResponse<List<Category>>(data, request.PageNumber, request.PageSize, totalCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao listar categorias");
                return new PagedResponse<List<Category>>(null, 500, "Ocorreu um erro ao buscar as categorias.");
            }
        }

        public async Task<Response<Category>> GetByIdAsync(CategoryGetByIdRequest request)
        {
            try
            {
                var category = await repository.GetByIdAsync(request.Id, user.UserId);
                return category is null
                    ? new Response<Category>(null, 404, "Categoria não encontrada.")
                    : new Response<Category>(category, 200, "Categoria encontrada.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao buscar categoria {CategoryId}", request.Id);
                return new Response<Category>(null, 500, "Ocorreu um erro ao buscar a categoria.");
            }
        }

        public async Task<Response<Category>> AddAsync(CategoryCreateRequest request)
        {
            try
            {
                var category = new Category
                {
                    Name = request.Name,
                    Description = request.Description,
                    UserId = user.UserId,
                };

                await repository.AddAsync(category);
                await repository.SaveChangesAsync();

                return new Response<Category>(category, 201, "Categoria criada com sucesso.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao criar categoria");
                return new Response<Category>(null, 500, "Ocorreu um erro ao criar a categoria.");
            }
        }

        public async Task<Response<Category>> UpdateAsync(CategoryUpdateRequest request)
        {
            try
            {
                var category = await repository.GetByIdAsync(request.Id, user.UserId);
                if (category is null)
                    return new Response<Category>(null, 404, "Categoria não encontrada.");

                category.Name = request.Name;
                category.Description = request.Description;

                repository.Update(category);
                await repository.SaveChangesAsync();

                return new Response<Category>(category, 200, "Categoria atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao atualizar categoria {CategoryId}", request.Id);
                return new Response<Category>(null, 500, "Ocorreu um erro ao atualizar a categoria.");
            }
        }

        public async Task<Response<Category>> DeleteAsync(CategoryDeleteRequest request)
        {
            try
            {
                var category = await repository.GetByIdAsync(request.Id, user.UserId);
                if (category is null)
                    return new Response<Category>(null, 404, "Categoria não encontrada.");

                repository.Delete(category);
                await repository.SaveChangesAsync();

                return new Response<Category>(category, 200, "Categoria deletada com sucesso.");
            }
            catch (DbUpdateException ex)
            {
                logger.LogWarning(ex, "Tentativa de excluir categoria {CategoryId} com transações vinculadas", request.Id);
                return new Response<Category>(null, 409, "Não é possível excluir uma categoria que possui transações vinculadas.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao deletar categoria {CategoryId}", request.Id);
                return new Response<Category>(null, 500, "Ocorreu um erro ao deletar a categoria.");
            }
        }
    }
}
