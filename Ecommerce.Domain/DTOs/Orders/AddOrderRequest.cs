using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Orders;

public class AddOrderRequest
{
    [Required(ErrorMessage = "El ID de usuario es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de usuario debe ser mayor a 0")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Los items del pedido son obligatorios")]
    [MinLength(1, ErrorMessage = "Debe haber al menos un item en el pedido")]
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();

    [Required(ErrorMessage = "La dirección de facturación es obligatoria")]
    [ValidateObject(ErrorMessage = "La dirección de facturación no es válida")]
    public AddressDto BillingAddress { get; set; } = null!;

    [Required(ErrorMessage = "La dirección de envío es obligatoria")]
    [ValidateObject(ErrorMessage = "La dirección de envío no es válida")]
    public AddressDto ShippingAddress { get; set; } = null!;

    [Required(ErrorMessage = "La información de pago es obligatoria")]
    [ValidateObject(ErrorMessage = "La información de pago no es válida")]
    public PaymentInfoDto PaymentInfo { get; set; } = null!;
}

// DTOs complementarios
public class OrderItemDto
{
    [Required(ErrorMessage = "El ID del producto es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser mayor a 0")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "El nombre del producto es obligatorio")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "El nombre del producto debe tener entre 1 y 200 caracteres")]
    public string ProductName { get; set; } = null!;

    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
    public int Quantity { get; set; }
}

public class AddressDto
{
    [Required(ErrorMessage = "La calle es obligatoria")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "La calle debe tener entre 1 y 200 caracteres")]
    public string Street { get; set; } = null!;

    [Required(ErrorMessage = "La ciudad es obligatoria")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "La ciudad debe tener entre 1 y 100 caracteres")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "El estado/provincia es obligatorio")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El estado debe tener entre 1 y 100 caracteres")]
    public string State { get; set; } = null!;

    [Required(ErrorMessage = "El código postal es obligatorio")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "El código postal debe tener entre 1 y 20 caracteres")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "El país es obligatorio")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El país debe tener entre 1 y 100 caracteres")]
    public string Country { get; set; } = null!;

    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(150, ErrorMessage = "El email no puede exceder 150 caracteres")]
    public string? Email { get; set; }

    [RegularExpression(@"^(\+\d{1,3})?\s?\(?\d{1,4}\)?[\s.-]?\d{3}[\s.-]?\d{4}$",
        ErrorMessage = "El formato del teléfono no es válido. Ejemplo: +34 612 345 678")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    public string? Phone { get; set; }

}

public class PaymentInfoDto
{
    [Required(ErrorMessage = "El método de pago es obligatorio")]
    public string Method { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El nombre del titular no puede exceder 100 caracteres")]
    public string CardHolderName { get; set; } = string.Empty;

    [StringLength(4, MinimumLength = 4, ErrorMessage = "Los últimos 4 dígitos deben tener exactamente 4 caracteres")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Debe contener solo 4 dígitos numéricos")]
    public string CardLastFour { get; set; } = string.Empty;

}

// Atributo personalizado para validar objetos complejos
[AttributeUsage(AttributeTargets.Property)]
public class ValidateObjectAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult(ErrorMessage ?? "El objeto no puede ser nulo");

        var results = new List<ValidationResult>();
        var context = new ValidationContext(value, null, null);

        if (!Validator.TryValidateObject(value, context, results, true))
        {
            return new ValidationResult(ErrorMessage ?? string.Join("; ", results.Select(r => r.ErrorMessage)));
        }

        return ValidationResult.Success;
    }
}