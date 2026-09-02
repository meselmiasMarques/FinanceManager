using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FinanceManager.Web.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace FinanceManager.Web.Services.Auth;

/// <summary>
/// Cliente dos endpoints <c>/auth/*</c>. Usa um <see cref="HttpClient"/> <b>sem</b> o
/// <see cref="AuthMessageHandler"/> (evita recursão no fluxo de refresh) e envia credenciais
/// (<c>include</c>) para que o cookie de refresh trafegue.
/// </summary>
public sealed class AuthApiClient(HttpClient http, TokenStore tokenStore)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public Task<ApiResponse<UserProfileModel>> RegisterAsync(RegisterFormModel form, CancellationToken ct = default)
        => SendAsync<UserProfileModel>(HttpMethod.Post, "auth/register", new
        {
            form.DisplayName,
            form.Email,
            form.Password
        }, auth: false, ct);

    public Task<ApiResponse<AuthTokensModel>> LoginAsync(LoginFormModel form, CancellationToken ct = default)
        => SendAsync<AuthTokensModel>(HttpMethod.Post, "auth/login", new
        {
            form.Email,
            form.Password,
            form.RememberMe
        }, auth: false, ct);

    public Task<ApiResponse<AuthTokensModel>> RefreshAsync(CancellationToken ct = default)
        => SendAsync<AuthTokensModel>(HttpMethod.Post, "auth/refresh", body: null, auth: false, ct);

    public Task<ApiResponse<object>> LogoutAsync(CancellationToken ct = default)
        => SendAsync<object>(HttpMethod.Post, "auth/logout", body: null, auth: false, ct);

    public Task<ApiResponse<UserProfileModel>> GetMeAsync(CancellationToken ct = default)
        => SendAsync<UserProfileModel>(HttpMethod.Get, "auth/me", body: null, auth: true, ct);

    public Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordFormModel form, CancellationToken ct = default)
        => SendAsync<object>(HttpMethod.Post, "auth/change-password", new
        {
            form.CurrentPassword,
            form.NewPassword
        }, auth: true, ct);

    private async Task<ApiResponse<T>> SendAsync<T>(
        HttpMethod method, string uri, object? body, bool auth, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, uri);
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        if (body is not null)
            request.Content = JsonContent.Create(body, options: JsonOptions);

        if (auth && !string.IsNullOrEmpty(tokenStore.AccessToken))
            request.Headers.Authorization = new("Bearer", tokenStore.AccessToken);

        try
        {
            using var response = await http.SendAsync(request, ct);

            if (response.StatusCode == HttpStatusCode.NoContent)
                return new ApiResponse<T> { Code = 204 };

            try
            {
                var envelope = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonOptions, ct);
                if (envelope is not null)
                {
                    if (envelope.Code == 0) envelope.Code = (int)response.StatusCode;
                    return envelope;
                }
            }
            catch (JsonException) { /* corpo não é o envelope (ex.: ProblemDetails) */ }

            return new ApiResponse<T>
            {
                Code = (int)response.StatusCode,
                Message = response.IsSuccessStatusCode ? string.Empty : "Falha na requisição."
            };
        }
        catch (HttpRequestException ex)
        {
            return new ApiResponse<T> { Code = 503, Message = $"Não foi possível contatar a API: {ex.Message}" };
        }
        catch (TaskCanceledException)
        {
            return new ApiResponse<T> { Code = 408, Message = "A requisição demorou demais." };
        }
    }
}
