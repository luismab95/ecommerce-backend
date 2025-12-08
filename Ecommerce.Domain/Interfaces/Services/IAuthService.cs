using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Interfaces.Services;

public interface IAuthService
{
    Task<IDictionary<string, object>> GetPayloadJwtTokenAsync(string token);
    bool ValidateToken(string token, bool validateLifetime);
    Task<string> GenerateTokenAsync(User user, int sessionId);
    Task<string> GenerateRefreshTokenAsync(User user);
    Task<string> GenerateResetPasswordTokenAsync(User user);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
