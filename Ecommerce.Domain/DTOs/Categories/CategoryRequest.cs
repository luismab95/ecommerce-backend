using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Categories;

public class CategoryRequest
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "La descripción debe tener entre 2 y 255 caracteres")]
    public string Description { get; set; } = string.Empty;
}
