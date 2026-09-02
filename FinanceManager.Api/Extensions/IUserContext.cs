using System.Security.Claims;

namespace FinanceManager.Extensions
{
    /// <summary>
    /// Inquilino (usuário) da requisição atual, derivado do token JWT.
    /// É a única fonte de verdade para "quem sou eu" — services, repositórios e o
    /// <c>Global Query Filter</c> do EF Core consomem esta abstração.
    /// </summary>
    public interface IUserContext
    {
        /// <summary>Id do usuário autenticado. Lança se não houver usuário autenticado.</summary>
        Guid UserId { get; }

        /// <summary>Id do usuário autenticado, ou <c>null</c> quando anônimo (uso no query filter).</summary>
        Guid? UserIdOrNull { get; }

        bool IsAuthenticated { get; }
    }

    public sealed class CurrentUserContext(IHttpContextAccessor accessor) : IUserContext
    {
        public Guid? UserIdOrNull
        {
            get
            {
                var principal = accessor.HttpContext?.User;
                if (principal?.Identity?.IsAuthenticated != true)
                    return null;

                var raw = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? principal.FindFirstValue("sub");

                return Guid.TryParse(raw, out var id) ? id : null;
            }
        }

        public bool IsAuthenticated => UserIdOrNull is not null;

        public Guid UserId => UserIdOrNull
            ?? throw new InvalidOperationException("Nenhum usuário autenticado na requisição atual.");
    }
}
