namespace Ecommerce.Domain.Entities;

public class Session
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string DeviceInfo { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public string RefreshToken { get; private set; } = string.Empty;
    public bool Revoked { get; private set; } = false;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public DateTime ExpiresAt { get; private set; } = DateTime.Now;
    public DateTime? RevokedAt { get; private set; }


    public virtual User? User { get; set; }

    private Session() { }

    public static Session Create(int userId, string deviceInfo, string ipAddress, string refreshToken, DateTime expiresAt)
    {
        return new Session
        {
            UserId = userId,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            RefreshToken = refreshToken,
            Revoked = false,
            CreatedAt = DateTime.Now,
            ExpiresAt = expiresAt,
        };
    }

    public static Session Update(Session session)
    {
        session.Revoked = true;
        session.RevokedAt = DateTime.UtcNow;
        return session;
    }
}