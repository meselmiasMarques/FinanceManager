using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Requests.Categories
{
    public class CategoryUpdateRequest : RequestBase
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O nome não pode passar de 50 caracteres.")]
        [MinLength(3, ErrorMessage = "O nome deve ter ao menos 3 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "A descrição não pode passar de 100 caracteres.")]
        public string Description { get; set; } = string.Empty;
    }
}
