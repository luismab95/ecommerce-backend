using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Products;

public class ProductRequest
{
    [Required(ErrorMessage = "El nombre del producto es requerido")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción del producto es requerida")]
    [StringLength(255, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 255 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio es requerido")]
    [Range(0.01, 100000, ErrorMessage = "El precio debe estar entre 0.01 y 100,000")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "El stock es requerido")]
    [Range(0, 100000, ErrorMessage = "El stock debe estar entre 0 y 100,000")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "La categoría es requerida")]
    public int CategoryId { get; set; } = 0;

    public bool Featured { get; set; } = false;
}

