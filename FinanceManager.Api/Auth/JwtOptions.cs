namespace FinanceManager.Auth
{
    /// <summary>Configuração do JWT de acesso. Seção <c>Jwt</c> em configuração; a
    /// <see cref="SigningKey"/> deve vir de user-secrets / variável de ambiente.</summary>
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";

        public string Issuer { get; set; } = "FinanceManager";
        public string Audience { get; set; } = "FinanceManager.Web";
        public string SigningKey { get; set; } = string.Empty;

        /// <summary>Validade do access token, em minutos (curto — renovado via refresh token).</summary>
        public int AccessTokenMinutes { get; set; } = 15;

        /// <summary>Validade do refresh token, em dias.</summary>
        public int RefreshTokenDays { get; set; } = 7;

        /// <summary>Validade do refresh token com "manter conectado", em dias.</summary>
        public int RefreshTokenRememberMeDays { get; set; } = 30;
    }
}
