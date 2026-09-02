namespace FinanceManager.Web.Models;

/// <summary>Espelha o envelope <c>Response&lt;T&gt;</c> da API.</summary>
public class ApiResponse<T>
{
    public T? Data { get; set; }
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool IsSuccess => Code is >= 200 and < 300;
}

/// <summary>Espelha o envelope <c>PagedResponse&lt;T&gt;</c> da API.</summary>
public class PagedApiResponse<T> : ApiResponse<T>
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
