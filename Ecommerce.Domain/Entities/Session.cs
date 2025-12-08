namespace Ecommerce.Domain.Entities;

public class Session
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string DeviceInfo { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public string RefreshToken { get; private set; } = string.Empty;
    public DateTime LoginAt { get; private set; }
    public DateTime? LogoutAt { get; private set; }
    public bool IsActive { get; private set; } = true;


    public virtual User? User { get; set; }

    private Session() { }

    public static Session Create(int userId, string deviceInfo, string ipAddress, string refreshToken, bool isActive)
    {
        return new Session
        {
            UserId = userId,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            RefreshToken = refreshToken,
            IsActive = isActive,
            LoginAt = DateTime.Now,
        };
    }

    public static Session Update(Session session)
    {
        session.IsActive = false;
        session.LogoutAt = DateTime.UtcNow;
        return session;
    }
}