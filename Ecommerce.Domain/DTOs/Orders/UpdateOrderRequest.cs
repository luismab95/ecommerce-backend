using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Orders;

public class UpdateOrderRequest
{
    [Required(ErrorMessage = "El Estado es requerido")]
    public string Status { get; set; } = string.Empty;
}
