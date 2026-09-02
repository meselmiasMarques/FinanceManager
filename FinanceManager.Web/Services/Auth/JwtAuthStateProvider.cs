using System.Security.Claims;
using System.Text.Json;
using FinanceManager.Web.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace FinanceManager.Web.Services.Auth;

/// <summary>
/// Estado de autenticação derivado do access token em memória. No primeiro acesso tenta uma
/// renovação silenciosa (o cookie de refresh sobrevive a recargas da página).
/// </summary>
public sealed class JwtAuthStateProvider(AuthApiClient authApi, TokenStore tokenStore) : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private bool _bootstrapped;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (tokenStore.HasValidToken)
            return BuildState(tokenStore.AccessToken!);

        if (!_bootstrapped)
        {
            _bootstrapped = true;
            if (await TryRefreshAsync())
                return BuildState(tokenStore.AccessToken!);
        }

        return Anonymous;
    }

    public void SignIn(AuthTokensModel tokens)
    {
        tokenStore.Set(tokens.AccessToken, tokens.ExpiresIn);
        _bootstrapped = true;
        NotifyAuthenticationStateChanged(Task.FromResult(BuildState(tokens.AccessToken)));
    }

    public void SignOutLocal()
    {
        tokenStore.Clear();
        _bootstrapped = true;
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    /// <summary>Renovação única (single-flight). Retorna <c>true</c> se há um access token válido ao final.</summary>
    public async Task<bool> TryRefreshAsync()
    {
        if (tokenStore.HasValidToken)
            return true;

        await _refreshLock.WaitAsync();
        try
        {
            if (tokenStore.HasValidToken)
                return true;

            var response = await authApi.RefreshAsync();
            if (response.IsSuccess && response.Data is not null)
            {
                tokenStore.Set(response.Data.AccessToken, response.Data.ExpiresIn);
                _bootstrapped = true;
                NotifyAuthenticationStateChanged(Task.FromResult(BuildState(response.Data.AccessToken)));
                return true;
            }

            tokenStore.Clear();
            return false;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private static AuthenticationState BuildState(string accessToken)
        => new(new ClaimsPrincipal(new ClaimsIdentity(ParseClaims(accessToken), authenticationType: "jwt", nameType: "name", roleType: "role")));

    private static IEnumerable<Claim> ParseClaims(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length < 2)
            return [];

        var json = Base64UrlDecode(parts[1]);
        var map = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        if (map is null)
            return [];

        var claims = new List<Claim>();
        foreach (var (key, value) in map)
        {
            if (value.ValueKind == JsonValueKind.Array)
                claims.AddRange(value.EnumerateArray().Select(v => new Claim(key, v.ToString())));
            else
                claims.Add(new Claim(key, value.ToString()));
        }
        return claims;
    }

    private static string Base64UrlDecode(string input)
    {
        var s = input.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }
        return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(s));
    }
}
