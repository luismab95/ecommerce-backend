namespace Ecommerce.Domain.DTOs.Auth;

public class JwtResponse
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpireHours { get; set; }
    public int RefreshExpireDays { get; set; }
    public int ResetPasswordExpireMinutes { get; set; }
}
