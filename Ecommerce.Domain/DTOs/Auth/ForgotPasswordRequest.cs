using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.DTOs.Auth;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(255, ErrorMessage = "El email no puede exceder 255 caracteres")]
    public string Email { get; set; } = string.Empty;

}
