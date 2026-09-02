using Microsoft.AspNetCore.Http;

namespace FinanceManager.Auth
{
    /// <summary>Configuração do cookie <c>HttpOnly</c> que carrega o refresh token. Seção <c>Auth</c>.</summary>
    public sealed class AuthCookieOptions
    {
        public const string SectionName = "Auth";

        public string RefreshCookieName { get; set; } = "fm_refresh";

        /// <summary>Caminho do cookie. Restringe o envio aos endpoints de <c>/auth</c>.</summary>
        public string RefreshCookiePath { get; set; } = "/auth";

        /// <summary>"Lax" | "Strict" | "None". Use "None" (com HTTPS) se a Web ficar em site distinto da API.</summary>
        public string SameSite { get; set; } = "Lax";

        public string? Domain { get; set; }

        /// <summary>Se null, decide pelo ambiente (Secure fora de Development).</summary>
        public bool? Secure { get; set; }

        public SameSiteMode SameSiteMode => SameSite.Trim().ToLowerInvariant() switch
        {
            "strict" => SameSiteMode.Strict,
            "none" => SameSiteMode.None,
            _ => SameSiteMode.Lax,
        };
    }
}
