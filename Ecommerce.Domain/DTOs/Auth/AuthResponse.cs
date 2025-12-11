using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.DTOs.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public dynamic? User { get; set; }
    public dynamic? ShoppingCart { get; set; }

}

