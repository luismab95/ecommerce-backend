using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.DTOs.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public dynamic? User { get; set; }

}

