using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Auth
{
    public sealed class RegisterRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 80 caracteres.")]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [StringLength(256, ErrorMessage = "O e-mail não pode passar de 256 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "A senha deve ter ao menos 8 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }

    public sealed class LoginRequest
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public sealed class ChangePasswordRequest
    {
        [Required(ErrorMessage = "A senha atual é obrigatória.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "A nova senha deve ter ao menos 8 caracteres.")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public sealed class AuthTokensResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";

        /// <summary>Validade do access token, em segundos.</summary>
        public int ExpiresIn { get; set; }

        public UserProfileResponse User { get; set; } = new();
    }

    public sealed class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
