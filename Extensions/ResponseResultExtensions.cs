using FinanceManager.Responses;

namespace FinanceManager.Extensions
{
    /// <summary>
    /// Converte o envelope <see cref="Response{T}"/> em um <see cref="IResult"/> cujo
    /// status HTTP reflete o <see cref="Response{T}.Code"/> (404 vira 404, 500 vira 500, etc.).
    /// </summary>
    public static class ResponseResultExtensions
    {
        public static IResult ToHttpResult<T>(this Response<T> response) where T : class
            => Results.Json(response, statusCode: response.Code);

        public static IResult ToCreatedResult<T>(this Response<T> response, Func<T, string> locationFactory)
            where T : class
            => response.IsSuccess && response.Data is not null
                ? Results.Created(locationFactory(response.Data), response)
                : response.ToHttpResult();
    }
}
