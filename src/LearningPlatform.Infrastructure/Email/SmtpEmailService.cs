using System.Net;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Shared.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LearningPlatform.Infrastructure.Email;

public class SmtpEmailService(
    IOptions<MailSettings> mailSettings,
    ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly MailSettings _settings = mailSettings.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.Password))
        {
            logger.LogWarning("SMTP password is not configured. Email to {To} was not sent.", message.To);
            throw new InvalidOperationException(
                "SMTP password is not configured. Set the MailSettings:Password environment variable or user secret.");
        }

        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        email.To.Add(MailboxAddress.Parse(message.To));
        email.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message.IsHtml ? message.Body : null,
            TextBody = message.IsHtml ? null : message.Body
        };

        email.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(_settings.UserName, _settings.Password, cancellationToken);
            await client.SendAsync(email, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Email sent to {To} with subject '{Subject}'.", message.To, message.Subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {To} with subject '{Subject}'.", message.To, message.Subject);
            throw;
        }
    }
}
