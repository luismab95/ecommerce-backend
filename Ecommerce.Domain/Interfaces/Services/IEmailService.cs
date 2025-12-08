using Ecommerce.Domain.DTOs.Email;

namespace Ecommerce.Domain.Interfaces.Services;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
}
