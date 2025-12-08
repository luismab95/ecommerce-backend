using Ecommerce.Domain.DTOs.Email;
using Ecommerce.Domain.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Ecommerce.Infrastructure.Services;

public class GmailSmtpEmailSender : IEmailService
{
    private readonly IConfiguration _config;
    private readonly IHostEnvironment _env;

    public GmailSmtpEmailSender(IConfiguration config, IHostEnvironment env)
    {
        _config = config;
        _env = env;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {

        var smtpUser = _config["Gmail:Username"]
                       ?? throw new InvalidOperationException("Gmail:Username no está configurado.");

        var smtpPass = _config["Gmail:AppPassword"]
                       ?? throw new InvalidOperationException("Gmail:AppPassword no está configurado.");

        var smtpHost = _config["Gmail:SmtpHost"]
                       ?? throw new InvalidOperationException("Gmail:SmtpHost no está configurado.");

        if (!int.TryParse(_config["Gmail:SmtpPort"], out var smtpPort))
            throw new InvalidOperationException("Gmail:SmtpPort no es un número válido.");

        if (message.To == null || !message.To.Any())
            throw new InvalidOperationException("No se especificaron destinatarios.");

        var email = new MimeMessage();

        // FROM
        email.From.Add(MailboxAddress.Parse(message.From ?? smtpUser));

        // TO
        foreach (var to in message.To)
            email.To.Add(MailboxAddress.Parse(to));

        // SUBJECT
        email.Subject = message.Subject ?? string.Empty;

        // BODY
        email.Body = new TextPart(message.IsHtml ? "html" : "plain")
        {
            Text = message.Body ?? string.Empty
        };

        // SMTP Client
        using var smtp = new SmtpClient();

        try
        {
            // Ignorar certificados solo en desarrollo
            if (_env.IsDevelopment())
                smtp.ServerCertificateValidationCallback = (_, _, _, _) => true;

            await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, ct)
                      .ConfigureAwait(false);

            await smtp.AuthenticateAsync(smtpUser, smtpPass, ct)
                      .ConfigureAwait(false);

            await smtp.SendAsync(email, ct)
                      .ConfigureAwait(false);
        }
        catch (Exception ex)
        {

            throw new InvalidOperationException("Error al enviar el correo electrónico.", ex);
        }
        finally
        {
            if (smtp.IsConnected)
            {
                await smtp.DisconnectAsync(true, ct)
                          .ConfigureAwait(false);
            }
        }
    }
}
