using FinanceManager.Requests;
using FinanceManager.Requests.Categories;
using FinanceManager.Services;

namespace FinanceManager.Extensions.Endpoints
{
    public class CategoryEndPoints : IEndPoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/categories").WithTags("Categorias");

            group.MapGet("/", async (
                ICategoryService service,
                int pageNumber = 1,
                int pageSize = PagedRequest.DefaultPageSize) =>
            {
                var result = await service.GetAllAsync(new CategoryGetAllRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                });
                return result.ToHttpResult();
            })
            .WithDescription("Lista as categorias do usuário (paginado).");

            group.MapGet("/{id:int}", async (ICategoryService service, int id) =>
            {
                var result = await service.GetByIdAsync(new CategoryGetByIdRequest { Id = id });
                return result.ToHttpResult();
            })
            .WithDescription("Recupera uma categoria pelo ID.");

            group.MapPost("/", async (ICategoryService service, CategoryCreateRequest request) =>
            {
                var result = await service.AddAsync(request);
                return result.ToCreatedResult(category => $"/categories/{category.Id}");
            })
            .WithDescription("Cria uma nova categoria.");

            group.MapPut("/{id:int}", async (ICategoryService service, int id, CategoryUpdateRequest request) =>
            {
                request.Id = id;
                var result = await service.UpdateAsync(request);
                return result.ToHttpResult();
            })
            .WithDescription("Edita uma categoria existente.");

            group.MapDelete("/{id:int}", async (ICategoryService service, int id) =>
            {
                var result = await service.DeleteAsync(new CategoryDeleteRequest { Id = id });
                return result.ToHttpResult();
            })
            .WithDescription("Remove uma categoria.");
        }
    }
}
