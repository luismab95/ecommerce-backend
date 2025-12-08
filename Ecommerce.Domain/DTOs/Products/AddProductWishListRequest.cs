using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Products;

public class AddProductWishListRequest
{
    [Required(ErrorMessage = "El ID del producto es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El producto debe tener un ID válido")]
    public int ProductId { get; set; }
}
