using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Web.Models;

public class CategoryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CategoryFormModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 50 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "A descrição não pode passar de 200 caracteres.")]
    public string Description { get; set; } = string.Empty;

    public bool IsNew => Id == 0;

    public static CategoryFormModel FromCategory(CategoryModel c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Description = c.Description
    };
}
