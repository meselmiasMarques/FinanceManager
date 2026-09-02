using Microsoft.AspNetCore.Identity;

namespace FinanceManager.Models
{
    /// <summary>
    /// Usuário/inquilino do sistema. Chave <see cref="Guid"/> (ADR-01).
    /// Cada usuário só enxerga suas próprias categorias, transações e dashboards.
    /// </summary>
    public class AppUser : IdentityUser<Guid>
    {
        /// <summary>Nome de exibição mostrado na interface (menu de usuário, saudações).</summary>
        public string DisplayName { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
