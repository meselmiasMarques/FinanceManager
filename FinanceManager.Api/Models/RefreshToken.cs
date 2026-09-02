namespace FinanceManager.Models
{
    /// <summary>
    /// Refresh token rotacionado. O valor em claro nunca é persistido — apenas o hash SHA-256
    /// (<see cref="TokenHash"/>). Reuso de um token já rotacionado revoga toda a cadeia do usuário.
    /// </summary>
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public AppUser? User { get; set; }

        /// <summary>Hash (Base64 de SHA-256) do valor entregue ao cliente.</summary>
        public string TokenHash { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string? CreatedByIp { get; set; }

        public DateTime? RevokedAtUtc { get; set; }
        public string? RevokedByIp { get; set; }

        /// <summary>Hash do token que substituiu este (rotação). Usado para detectar reuso.</summary>
        public string? ReplacedByTokenHash { get; set; }

        public bool IsActive => RevokedAtUtc is null && DateTime.UtcNow < ExpiresAtUtc;
    }
}
