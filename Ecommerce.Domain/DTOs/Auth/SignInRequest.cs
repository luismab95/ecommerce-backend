using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ecommerce.Domain.DTOs.Auth;

public class SignInRequest
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(255, ErrorMessage = "El email no puede exceder 255 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
       ErrorMessage = "La contraseña debe contener mayúsculas, minúsculas, números y caracteres especiales")]
    public string Password { get; set; } = string.Empty;

    [JsonIgnore]
    public string Ip { get; set; } = string.Empty;
    [JsonIgnore]
    public string DeviceInfo { get; set; } = string.Empty;


}
