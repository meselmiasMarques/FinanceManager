using FinanceManager.Auth;
using FinanceManager.Responses;
using Microsoft.Extensions.Options;

namespace FinanceManager.Extensions.Endpoints
{
    public class AuthEndPoints : IEndPoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/auth").WithTags("Autenticação");

            group.MapPost("/register", async (RegisterRequest request, IAuthService auth) =>
            {
                var result = await auth.RegisterAsync(request);
                return result.ToCreatedResult(u => $"/auth/users/{u.Id}");
            })
            .AllowAnonymous()
            .RequireRateLimiting("auth")
            .WithDescription("Cria uma nova conta.");

            group.MapPost("/login", async (
                LoginRequest request, IAuthService auth, HttpContext http,
                IOptions<AuthCookieOptions> cookie, IWebHostEnvironment env) =>
            {
                var outcome = await auth.LoginAsync(request, ClientIp(http));
                return WriteOutcome(outcome, http, cookie.Value, env);
            })
            .AllowAnonymous()
            .RequireRateLimiting("auth")
            .WithDescription("Autentica e emite access token (corpo) + refresh token (cookie HttpOnly).");

            group.MapPost("/refresh", async (
                IAuthService auth, HttpContext http,
                IOptions<AuthCookieOptions> cookie, IWebHostEnvironment env) =>
            {
                var raw = http.Request.Cookies[cookie.Value.RefreshCookieName];
                var outcome = await auth.RefreshAsync(raw, ClientIp(http));
                if (!outcome.IsSuccess)
                    ClearRefreshCookie(http, cookie.Value, env);
                return WriteOutcome(outcome, http, cookie.Value, env);
            })
            .AllowAnonymous()
            .RequireRateLimiting("auth")
            .WithDescription("Rotaciona o refresh token do cookie e devolve um novo access token.");

            group.MapPost("/logout", async (
                IAuthService auth, HttpContext http,
                IOptions<AuthCookieOptions> cookie, IWebHostEnvironment env) =>
            {
                var raw = http.Request.Cookies[cookie.Value.RefreshCookieName];
                await auth.LogoutAsync(raw, ClientIp(http));
                ClearRefreshCookie(http, cookie.Value, env);
                return Results.NoContent();
            })
            .AllowAnonymous()
            .WithDescription("Revoga o refresh token atual e limpa o cookie.");

            group.MapPost("/change-password", async (
                ChangePasswordRequest request, IAuthService auth, IUserContext user,
                HttpContext http, IOptions<AuthCookieOptions> cookie) =>
            {
                var raw = http.Request.Cookies[cookie.Value.RefreshCookieName];
                var (code, message) = await auth.ChangePasswordAsync(user.UserId, raw, request, ClientIp(http));
                return code == 204
                    ? Results.NoContent()
                    : Results.Json(new { data = (object?)null, code, message }, statusCode: code);
            })
            .RequireAuthorization()
            .RequireRateLimiting("auth")
            .WithDescription("Altera a senha do usuário autenticado e encerra as demais sessões.");

            group.MapGet("/me", async (IAuthService auth, IUserContext user) =>
            {
                var result = await auth.GetProfileAsync(user.UserId);
                return result.ToHttpResult();
            })
            .RequireAuthorization()
            .WithDescription("Perfil do usuário da sessão atual.");
        }

        private static string? ClientIp(HttpContext http)
            => http.Connection.RemoteIpAddress?.ToString();

        private static IResult WriteOutcome(AuthOutcome outcome, HttpContext http, AuthCookieOptions opt, IWebHostEnvironment env)
        {
            if (outcome.IsSuccess && outcome.RefreshToken is not null && outcome.RefreshExpiresAtUtc is not null)
                SetRefreshCookie(http, opt, env, outcome.RefreshToken, outcome.RefreshExpiresAtUtc.Value);

            var body = new Response<AuthTokensResponse>(outcome.Tokens, outcome.Code, outcome.Message);
            return Results.Json(body, statusCode: outcome.Code);
        }

        private static void SetRefreshCookie(HttpContext http, AuthCookieOptions opt, IWebHostEnvironment env, string token, DateTime expiresUtc)
            => http.Response.Cookies.Append(opt.RefreshCookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = opt.Secure ?? !env.IsDevelopment(),
                SameSite = opt.SameSiteMode,
                Path = opt.RefreshCookiePath,
                Domain = opt.Domain,
                Expires = new DateTimeOffset(expiresUtc, TimeSpan.Zero),
                IsEssential = true,
            });

        private static void ClearRefreshCookie(HttpContext http, AuthCookieOptions opt, IWebHostEnvironment env)
            => http.Response.Cookies.Delete(opt.RefreshCookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = opt.Secure ?? !env.IsDevelopment(),
                SameSite = opt.SameSiteMode,
                Path = opt.RefreshCookiePath,
                Domain = opt.Domain,
            });
    }
}
