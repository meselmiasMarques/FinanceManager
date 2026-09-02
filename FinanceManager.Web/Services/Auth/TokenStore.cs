namespace FinanceManager.Web.Services.Auth;

/// <summary>
/// Guarda o access token <b>apenas em memória</b> (ADR-04). O refresh token vive num
/// cookie <c>HttpOnly</c> e nunca é acessível ao JavaScript/WASM.
/// </summary>
public sealed class TokenStore
{
    public string? AccessToken { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    /// <summary>Considera expirado 30s antes, para dar folga à renovação.</summary>
    public bool HasValidToken =>
        !string.IsNullOrEmpty(AccessToken) && DateTimeOffset.UtcNow < ExpiresAt.AddSeconds(-30);

    public void Set(string accessToken, int expiresInSeconds)
    {
        AccessToken = accessToken;
        ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);
    }

    public void Clear()
    {
        AccessToken = null;
        ExpiresAt = default;
    }
}
