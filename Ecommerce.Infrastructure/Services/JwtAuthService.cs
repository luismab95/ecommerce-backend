using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Interfaces.Services;
using Ecommerce.Domain.DTOs.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Infrastructure.Services;

public class JwtAuthService : IAuthService
{
    private readonly JwtResponse _jwtOptions;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtAuthService(IOptions<JwtResponse> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public Task<string> GenerateTokenAsync(User user, int sessionId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, sessionId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.Role, "ACCESS_TOKEN")
        };

        return Task.FromResult(CreateJwtToken(claims, TimeSpan.FromHours(_jwtOptions.ExpireHours)));
    }

    public Task<string> GenerateRefreshTokenAsync(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.Role, "REFRESH_TOKEN")
        };

        return Task.FromResult(CreateJwtToken(claims, TimeSpan.FromDays(_jwtOptions.RefreshExpireDays)));
    }

    public Task<string> GenerateResetPasswordTokenAsync(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.Role, "RESET_PASSWORD")
        };

        return Task.FromResult(CreateJwtToken(
            claims,
            TimeSpan.FromMinutes(_jwtOptions.ResetPasswordExpireMinutes)
        ));
    }

    public bool ValidateToken(string token, bool validateLifetime)
    {
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);

        var parameters = new TokenValidationParameters
        {
            ValidIssuer = string.IsNullOrEmpty(_jwtOptions.Issuer) ? null : _jwtOptions.Issuer,
            ValidAudience = string.IsNullOrEmpty(_jwtOptions.Audience) ? null : _jwtOptions.Audience,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            _tokenHandler.ValidateToken(token, parameters, out _);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public Task<IDictionary<string, object>> GetPayloadJwtTokenAsync(string token)
    {
        if (!_tokenHandler.CanReadToken(token))
            throw new InvalidOperationException("Token inválido o con formato incorrecto.");

        var jwt = _tokenHandler.ReadJwtToken(token);

        var payload = jwt.Payload.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return Task.FromResult((IDictionary<string, object>)payload);

    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    // ============================================================
    // MÉTODO PRIVADO REUTILIZABLE PARA GENERAR TOKENS
    // ============================================================

    private string CreateJwtToken(IEnumerable<Claim> claims, TimeSpan duration)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: string.IsNullOrEmpty(_jwtOptions.Issuer) ? null : _jwtOptions.Issuer,
            audience: string.IsNullOrEmpty(_jwtOptions.Audience) ? null : _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(duration),
            signingCredentials: creds
        );

        return _tokenHandler.WriteToken(token);
    }
}
