using System.Net;
using System.Net.Http.Headers;
using FinanceManager.Web.Services.Auth;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace FinanceManager.Web.Services.Auth;

/// <summary>
/// Anexa o <c>Bearer</c> às chamadas de API e, num <c>401</c>, tenta renovar a sessão uma vez
/// e repetir a requisição. Falhando a renovação, marca a sessão como encerrada.
/// </summary>
public sealed class AuthMessageHandler(JwtAuthStateProvider auth, TokenStore tokenStore) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        if (!tokenStore.HasValidToken)
            await auth.TryRefreshAsync();

        if (!string.IsNullOrEmpty(tokenStore.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenStore.AccessToken);

        var response = await base.SendAsync(request, ct);
        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        // 401: uma tentativa de renovação + repetição.
        response.Dispose();

        if (await auth.TryRefreshAsync() && !string.IsNullOrEmpty(tokenStore.AccessToken))
        {
            var retry = await CloneAsync(request);
            retry.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            retry.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenStore.AccessToken);
            return await base.SendAsync(retry, ct);
        }

        auth.SignOutLocal();
        return new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = request };
    }

    private static async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage source)
    {
        var clone = new HttpRequestMessage(source.Method, source.RequestUri);

        if (source.Content is not null)
        {
            var bytes = await source.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(bytes);
            foreach (var header in source.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        foreach (var header in source.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        foreach (var (key, value) in source.Options)
            ((IDictionary<string, object?>)clone.Options)[key] = value;

        return clone;
    }
}
