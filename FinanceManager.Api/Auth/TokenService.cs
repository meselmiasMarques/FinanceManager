using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinanceManager.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FinanceManager.Auth
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAtUtc) CreateAccessToken(AppUser user);

        /// <summary>Gera um refresh token opaco. Retorna o valor em claro (vai para o cookie)
        /// e o hash (persistido).</summary>
        (string Token, string Hash) CreateRefreshToken();

        string Hash(string token);
    }

    public sealed class TokenService(IOptions<JwtOptions> options) : ITokenService
    {
        private readonly JwtOptions _opt = options.Value;

        public (string Token, DateTime ExpiresAtUtc) CreateAccessToken(AppUser user)
        {
            var expires = DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new("name", user.DisplayName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        public (string Token, string Hash) CreateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(bytes);
            return (token, Hash(token));
        }

        public string Hash(string token)
        {
            var digest = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(digest);
        }
    }
}
