using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Orders;

public class CreateOrUpdateShoppingCartRequest
{
    [Required(ErrorMessage = "El UserId es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El UserId debe ser mayor a 0.")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "La lista de Items es obligatoria.")]
    public List<CartItemRequest> Items { get; set; } = new();

}

public class CartItemRequest
{
    [Required(ErrorMessage = "El ProductId es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ProductId debe ser mayor a 0.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Quantity { get; set; }
}