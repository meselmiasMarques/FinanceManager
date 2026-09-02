using FinanceManager.Web.Models;

namespace FinanceManager.Web.Services;

public class TransactionApiClient(HttpClient http) : ApiClientBase(http)
{
    private const string BasePath = "transactions";

    public Task<PagedApiResponse<List<TransactionModel>>> GetAllAsync(int pageNumber = 1, int pageSize = 25, CancellationToken ct = default)
        => SendPagedAsync<List<TransactionModel>>(
            Json(HttpMethod.Get, $"{BasePath}/?pageNumber={pageNumber}&pageSize={pageSize}"), ct);

    public Task<ApiResponse<TransactionModel>> GetByIdAsync(int id, CancellationToken ct = default)
        => SendAsync<TransactionModel>(Json(HttpMethod.Get, $"{BasePath}/{id}"), ct);

    public Task<ApiResponse<TransactionModel>> CreateAsync(TransactionFormModel form, CancellationToken ct = default)
        => SendAsync<TransactionModel>(Json(HttpMethod.Post, $"{BasePath}/", ToPayload(form)), ct);

    public Task<ApiResponse<TransactionModel>> UpdateAsync(TransactionFormModel form, CancellationToken ct = default)
        => SendAsync<TransactionModel>(Json(HttpMethod.Put, $"{BasePath}/{form.Id}", ToPayload(form)), ct);

    public Task<ApiResponse<TransactionModel>> DeleteAsync(int id, CancellationToken ct = default)
        => SendAsync<TransactionModel>(Json(HttpMethod.Delete, $"{BasePath}/{id}"), ct);

    private static object ToPayload(TransactionFormModel form) => new
    {
        form.Title,
        Type = (int)form.Type,
        form.Amount,
        form.CategoryId
    };
}
