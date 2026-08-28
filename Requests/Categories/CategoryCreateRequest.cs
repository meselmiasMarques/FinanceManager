using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Requests.Categories
{
    public class CategoryCreateRequest
    {
        [Required(ErrorMessage = "The name is required")]
        [MaxLength(50, ErrorMessage = "The name cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "The name must be at least 3 characters long")]
        public string Name { get; set; }

        [MaxLength(100, ErrorMessage = "The description cannot exceed 100 characters")]
        public string Description { get; set; }
    }
}
