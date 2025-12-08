using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Users;


public class UpdateUserAddressRequest
{
    [Required(ErrorMessage = "La dirección de envío es requerida")]
    [ValidateObject]
    public Address ShippingAddress { get; set; } = null!;

    [Required(ErrorMessage = "La dirección de facturación es requerida")]
    [ValidateObject]
    public Address BillingAddress { get; set; } = null!;
}

public class Address
{
    [Required(ErrorMessage = "La calle es requerida")]
    [StringLength(200, ErrorMessage = "La calle no puede exceder los 200 caracteres")]
    public string Street { get; set; } = null!;

    [Required(ErrorMessage = "La ciudad es requerida")]
    [StringLength(100, ErrorMessage = "La ciudad no puede exceder los 100 caracteres")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "El estado/provincia es requerido")]
    [StringLength(100, ErrorMessage = "El estado no puede exceder los 100 caracteres")]
    public string State { get; set; } = null!;

    [Required(ErrorMessage = "El código postal es requerido")]
    [StringLength(20, ErrorMessage = "El código postal no puede exceder los 20 caracteres")]
    [RegularExpression(@"^\d{4,5}(-\d{4})?$", ErrorMessage = "Formato de código postal inválido")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "El país es requerido")]
    [StringLength(100, ErrorMessage = "El país no puede exceder los 100 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El país solo puede contener letras")]
    public string Country { get; set; } = null!;
}

// Custom attribute para validar objetos complejos
[AttributeUsage(AttributeTargets.Property)]
public class ValidateObjectAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success!;

        var results = new List<ValidationResult>();
        var context = new ValidationContext(value, null, null);

        Validator.TryValidateObject(value, context, results, true);

        if (results.Count != 0)
        {
            var compositeResults = new CompositeValidationResult(
                $"{validationContext.DisplayName} tiene errores de validación:",
                validationContext.MemberName!);

            results.ForEach(compositeResults.AddResult);

            return compositeResults;
        }

        return ValidationResult.Success!;
    }
}

public class CompositeValidationResult : ValidationResult
{
    private readonly List<ValidationResult> _results = new();

    public IEnumerable<ValidationResult> Results => _results;

    public CompositeValidationResult(string errorMessage, string memberName)
        : base(errorMessage, new[] { memberName }) { }

    public void AddResult(ValidationResult validationResult)
    {
        _results.Add(validationResult);
    }
}
