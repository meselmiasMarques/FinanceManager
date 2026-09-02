using FinanceManager.Data;
using FinanceManager.Models;
using FinanceManager.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FinanceManager.Auth
{
    /// <summary>Resultado de um fluxo que emite sessão. O <see cref="RefreshToken"/> em claro
    /// nunca vai para o corpo da resposta — o endpoint o coloca num cookie <c>HttpOnly</c>.</summary>
    public sealed record AuthOutcome
    {
        public int Code { get; init; }
        public string Message { get; init; } = string.Empty;
        public AuthTokensResponse? Tokens { get; init; }
        public string? RefreshToken { get; init; }
        public DateTime? RefreshExpiresAtUtc { get; init; }
        public bool IsSuccess => Code is >= 200 and < 300;

        public static AuthOutcome Fail(int code, string message) => new() { Code = code, Message = message };
    }

    public interface IAuthService
    {
        Task<Response<UserProfileResponse>> RegisterAsync(RegisterRequest request);
        Task<AuthOutcome> LoginAsync(LoginRequest request, string? ip);
        Task<AuthOutcome> RefreshAsync(string? rawRefreshToken, string? ip);
        Task LogoutAsync(string? rawRefreshToken, string? ip);
        Task<(int Code, string Message)> ChangePasswordAsync(Guid userId, string? currentRawRefreshToken, ChangePasswordRequest request, string? ip);
        Task<Response<UserProfileResponse>> GetProfileAsync(Guid userId);
    }

    public sealed class AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        AppDbContext db,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions,
        ILogger<AuthService> logger) : IAuthService
    {
        private readonly JwtOptions _opt = jwtOptions.Value;

        public async Task<Response<UserProfileResponse>> RegisterAsync(RegisterRequest request)
        {
            var email = request.Email.Trim();

            if (await userManager.FindByEmailAsync(email) is not null)
                return new Response<UserProfileResponse>(null, 409, "E-mail já cadastrado.");

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                DisplayName = request.DisplayName.Trim(),
                EmailConfirmed = true,
                CreatedAtUtc = DateTime.UtcNow,
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Code.Contains("Duplicate", StringComparison.OrdinalIgnoreCase)))
                    return new Response<UserProfileResponse>(null, 409, "E-mail já cadastrado.");

                return new Response<UserProfileResponse>(null, 400, TranslateErrors(result));
            }

            logger.LogInformation("Nova conta criada: {UserId}", user.Id);
            return new Response<UserProfileResponse>(Profile(user), 201, "Conta criada com sucesso.");
        }

        public async Task<AuthOutcome> LoginAsync(LoginRequest request, string? ip)
        {
            var user = await userManager.FindByEmailAsync(request.Email.Trim());
            if (user is null)
            {
                // Custo semelhante ao caminho de sucesso para não facilitar enumeração de e-mails.
                await Task.Delay(Random.Shared.Next(40, 90));
                return AuthOutcome.Fail(401, "E-mail ou senha inválidos.");
            }

            var check = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (check.IsLockedOut)
            {
                logger.LogWarning("Login bloqueado (lockout) para {UserId}", user.Id);
                return AuthOutcome.Fail(423, "Conta temporariamente bloqueada por tentativas inválidas. Tente novamente mais tarde.");
            }
            if (!check.Succeeded)
                return AuthOutcome.Fail(401, "E-mail ou senha inválidos.");

            logger.LogInformation("Login OK: {UserId}", user.Id);
            return await IssueSessionAsync(user, request.RememberMe, ip);
        }

        public async Task<AuthOutcome> RefreshAsync(string? rawRefreshToken, string? ip)
        {
            if (string.IsNullOrWhiteSpace(rawRefreshToken))
                return AuthOutcome.Fail(401, "Sessão inválida.");

            var hash = tokenService.Hash(rawRefreshToken);
            var stored = await db.RefreshTokens.Include(r => r.User)
                .FirstOrDefaultAsync(r => r.TokenHash == hash);

            if (stored is null)
                return AuthOutcome.Fail(401, "Sessão inválida.");

            if (stored.RevokedAtUtc is not null)
            {
                // Token já rotacionado sendo reapresentado => possível roubo. Revoga toda a cadeia.
                await RevokeAllActiveAsync(stored.UserId, ip);
                logger.LogWarning("Reuso de refresh token detectado para {UserId} — cadeia revogada", stored.UserId);
                return AuthOutcome.Fail(401, "Sessão inválida.");
            }

            if (DateTime.UtcNow >= stored.ExpiresAtUtc)
                return AuthOutcome.Fail(401, "Sessão expirada.");

            if (stored.User is null)
                return AuthOutcome.Fail(401, "Sessão inválida.");

            var (newRaw, newHash) = tokenService.CreateRefreshToken();

            stored.RevokedAtUtc = DateTime.UtcNow;
            stored.RevokedByIp = ip;
            stored.ReplacedByTokenHash = newHash;

            var rotated = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = stored.UserId,
                TokenHash = newHash,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = stored.ExpiresAtUtc, // preserva o teto absoluto da sessão
                CreatedByIp = ip,
            };
            db.RefreshTokens.Add(rotated);

            var (access, accessExp) = tokenService.CreateAccessToken(stored.User);
            await db.SaveChangesAsync();

            return new AuthOutcome
            {
                Code = 200,
                Message = "Sessão renovada.",
                Tokens = BuildTokens(access, accessExp, stored.User),
                RefreshToken = newRaw,
                RefreshExpiresAtUtc = rotated.ExpiresAtUtc,
            };
        }

        public async Task LogoutAsync(string? rawRefreshToken, string? ip)
        {
            if (string.IsNullOrWhiteSpace(rawRefreshToken))
                return;

            var hash = tokenService.Hash(rawRefreshToken);
            var stored = await db.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == hash);
            if (stored is { RevokedAtUtc: null })
            {
                stored.RevokedAtUtc = DateTime.UtcNow;
                stored.RevokedByIp = ip;
                await db.SaveChangesAsync();
            }
        }

        public async Task<(int Code, string Message)> ChangePasswordAsync(
            Guid userId, string? currentRawRefreshToken, ChangePasswordRequest request, string? ip)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return (401, "Sessão inválida.");

            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Code == "PasswordMismatch"))
                    return (400, "A senha atual está incorreta.");
                return (400, TranslateErrors(result));
            }

            // Encerra as demais sessões; mantém a atual (identificada pelo cookie).
            var keepHash = string.IsNullOrWhiteSpace(currentRawRefreshToken)
                ? null
                : tokenService.Hash(currentRawRefreshToken);

            var others = await db.RefreshTokens
                .Where(r => r.UserId == userId && r.RevokedAtUtc == null && r.TokenHash != keepHash)
                .ToListAsync();
            foreach (var t in others)
            {
                t.RevokedAtUtc = DateTime.UtcNow;
                t.RevokedByIp = ip;
            }
            await db.SaveChangesAsync();

            logger.LogInformation("Senha alterada: {UserId} ({Count} sessões encerradas)", userId, others.Count);
            return (204, "Senha alterada com sucesso.");
        }

        public async Task<Response<UserProfileResponse>> GetProfileAsync(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            return user is null
                ? new Response<UserProfileResponse>(null, 401, "Sessão inválida.")
                : new Response<UserProfileResponse>(Profile(user), 200, "OK.");
        }

        // ---- helpers -----------------------------------------------------------

        private async Task<AuthOutcome> IssueSessionAsync(AppUser user, bool rememberMe, string? ip)
        {
            var (newRaw, newHash) = tokenService.CreateRefreshToken();
            var days = rememberMe ? _opt.RefreshTokenRememberMeDays : _opt.RefreshTokenDays;

            var refresh = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = newHash,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(days),
                CreatedByIp = ip,
            };
            db.RefreshTokens.Add(refresh);

            var (access, accessExp) = tokenService.CreateAccessToken(user);
            await db.SaveChangesAsync();

            return new AuthOutcome
            {
                Code = 200,
                Message = "Autenticado.",
                Tokens = BuildTokens(access, accessExp, user),
                RefreshToken = newRaw,
                RefreshExpiresAtUtc = refresh.ExpiresAtUtc,
            };
        }

        private async Task RevokeAllActiveAsync(Guid userId, string? ip)
        {
            var actives = await db.RefreshTokens
                .Where(r => r.UserId == userId && r.RevokedAtUtc == null)
                .ToListAsync();
            foreach (var t in actives)
            {
                t.RevokedAtUtc = DateTime.UtcNow;
                t.RevokedByIp = ip;
            }
            await db.SaveChangesAsync();
        }

        private AuthTokensResponse BuildTokens(string access, DateTime accessExp, AppUser user) => new()
        {
            AccessToken = access,
            TokenType = "Bearer",
            ExpiresIn = Math.Max(0, (int)(accessExp - DateTime.UtcNow).TotalSeconds),
            User = Profile(user),
        };

        private static UserProfileResponse Profile(AppUser user) => new()
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email ?? string.Empty,
        };

        private static string TranslateErrors(IdentityResult result)
        {
            var messages = result.Errors.Select(e => e.Code switch
            {
                "PasswordTooShort" => "A senha é muito curta.",
                "PasswordRequiresDigit" => "A senha precisa de ao menos um número.",
                "PasswordRequiresLower" => "A senha precisa de ao menos uma letra minúscula.",
                "PasswordRequiresUpper" => "A senha precisa de ao menos uma letra maiúscula.",
                "PasswordRequiresNonAlphanumeric" => "A senha precisa de ao menos um caractere especial.",
                "PasswordRequiresUniqueChars" => "A senha precisa de mais caracteres distintos.",
                "DuplicateUserName" or "DuplicateEmail" => "E-mail já cadastrado.",
                "InvalidEmail" => "Informe um e-mail válido.",
                _ => e.Description,
            }).Distinct();

            return string.Join(" ", messages);
        }
    }
}
