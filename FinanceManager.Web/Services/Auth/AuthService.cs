using FinanceManager.Web.Models;

namespace FinanceManager.Web.Services.Auth;

public readonly record struct AuthActionResult(bool Succeeded, string? Error)
{
    public static readonly AuthActionResult Ok = new(true, null);
    public static AuthActionResult Fail(string error) => new(false, error);
}

/// <summary>Fachada usada pelas telas: login, cadastro, logout e troca de senha.</summary>
public sealed class AuthService(AuthApiClient api, JwtAuthStateProvider provider)
{
    public async Task<AuthActionResult> LoginAsync(LoginFormModel form)
    {
        var response = await api.LoginAsync(form);
        if (response.IsSuccess && response.Data is not null)
        {
            provider.SignIn(response.Data);
            return AuthActionResult.Ok;
        }
        return AuthActionResult.Fail(Message(response.Message, "Não foi possível entrar."));
    }

    public async Task<AuthActionResult> RegisterAsync(RegisterFormModel form)
    {
        var response = await api.RegisterAsync(form);
        if (!response.IsSuccess)
            return AuthActionResult.Fail(Message(response.Message, "Não foi possível criar a conta."));

        // Login automático com as credenciais recém-criadas.
        return await LoginAsync(new LoginFormModel { Email = form.Email, Password = form.Password });
    }

    public async Task LogoutAsync()
    {
        try { await api.LogoutAsync(); }
        catch { /* logout é best-effort no servidor; o estado local é o que importa */ }
        provider.SignOutLocal();
    }

    public async Task<AuthActionResult> ChangePasswordAsync(ChangePasswordFormModel form)
    {
        var response = await api.ChangePasswordAsync(form);
        return response.IsSuccess
            ? AuthActionResult.Ok
            : AuthActionResult.Fail(Message(response.Message, "Não foi possível alterar a senha."));
    }

    private static string Message(string? fromApi, string fallback)
        => string.IsNullOrWhiteSpace(fromApi) ? fallback : fromApi!;
}
