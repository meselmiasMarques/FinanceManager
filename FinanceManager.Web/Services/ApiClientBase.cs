using System.Net.Http.Json;
using System.Text.Json;
using FinanceManager.Web.Models;

namespace FinanceManager.Web.Services;

public abstract class ApiClientBase(HttpClient http)
{
    protected HttpClient Http { get; } = http;

    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    protected async Task<ApiResponse<T>> SendAsync<T>(HttpRequestMessage request, CancellationToken ct = default)
        => await ReadAsync<ApiResponse<T>, T>(request, ct);

    protected async Task<PagedApiResponse<T>> SendPagedAsync<T>(HttpRequestMessage request, CancellationToken ct = default)
        => await ReadAsync<PagedApiResponse<T>, T>(request, ct);

    private async Task<TEnvelope> ReadAsync<TEnvelope, T>(HttpRequestMessage request, CancellationToken ct)
        where TEnvelope : ApiResponse<T>, new()
    {
        try
        {
            using var response = await Http.SendAsync(request, ct);

            try
            {
                var envelope = await response.Content.ReadFromJsonAsync<TEnvelope>(JsonOptions, ct);
                if (envelope is not null)
                {
                    if (envelope.Code == 0)
                        envelope.Code = (int)response.StatusCode;
                    return envelope;
                }
            }
            catch (JsonException)
            {
                // Corpo não é o envelope esperado (ex.: ProblemDetails). Cai no fallback abaixo.
            }

            return new TEnvelope
            {
                Code = (int)response.StatusCode,
                Message = response.IsSuccessStatusCode
                    ? "Resposta inesperada do servidor."
                    : $"Falha na requisição ({(int)response.StatusCode})."
            };
        }
        catch (HttpRequestException ex)
        {
            return new TEnvelope { Code = 503, Message = $"Não foi possível contatar a API: {ex.Message}" };
        }
        catch (TaskCanceledException)
        {
            return new TEnvelope { Code = 408, Message = "A requisição demorou demais e foi cancelada." };
        }
    }

    protected static HttpRequestMessage Json(HttpMethod method, string uri, object? body = null)
    {
        var request = new HttpRequestMessage(method, uri);
        if (body is not null)
            request.Content = JsonContent.Create(body, options: JsonOptions);
        return request;
    }
}
