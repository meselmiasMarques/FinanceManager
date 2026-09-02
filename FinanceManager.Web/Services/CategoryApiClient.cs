using FinanceManager.Web.Models;

namespace FinanceManager.Web.Services;

public class CategoryApiClient(HttpClient http) : ApiClientBase(http)
{
    private const string BasePath = "categories";

    public Task<PagedApiResponse<List<CategoryModel>>> GetAllAsync(int pageNumber = 1, int pageSize = 25, CancellationToken ct = default)
        => SendPagedAsync<List<CategoryModel>>(
            Json(HttpMethod.Get, $"{BasePath}/?pageNumber={pageNumber}&pageSize={pageSize}"), ct);

    public Task<ApiResponse<CategoryModel>> GetByIdAsync(int id, CancellationToken ct = default)
        => SendAsync<CategoryModel>(Json(HttpMethod.Get, $"{BasePath}/{id}"), ct);

    public Task<ApiResponse<CategoryModel>> CreateAsync(CategoryFormModel form, CancellationToken ct = default)
        => SendAsync<CategoryModel>(Json(HttpMethod.Post, $"{BasePath}/", new
        {
            form.Name,
            form.Description
        }), ct);

    public Task<ApiResponse<CategoryModel>> UpdateAsync(CategoryFormModel form, CancellationToken ct = default)
        => SendAsync<CategoryModel>(Json(HttpMethod.Put, $"{BasePath}/{form.Id}", new
        {
            form.Name,
            form.Description
        }), ct);

    public Task<ApiResponse<CategoryModel>> DeleteAsync(int id, CancellationToken ct = default)
        => SendAsync<CategoryModel>(Json(HttpMethod.Delete, $"{BasePath}/{id}"), ct);
}
