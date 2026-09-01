namespace FinanceManager.Extensions
{
    /// <summary>
    /// Abstração do usuário autenticado da requisição atual.
    /// Enquanto autenticação não é implementada, retorna um usuário fixo (ver <see cref="CurrentUserContext"/>).
    /// Quando o login entrar, basta trocar a implementação registrada no DI.
    /// </summary>
    public interface IUserContext
    {
        int UserId { get; }
    }

    public class CurrentUserContext : IUserContext
    {
        // TODO: derivar do token/Identity quando a etapa de segurança for implementada.
        public int UserId => 1;
    }
}
