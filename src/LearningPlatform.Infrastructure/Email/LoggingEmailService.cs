using LearningPlatform.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.Infrastructure.Email;

/// <summary>
/// Default IEmailService implementation: writes the message to the log instead of
/// dispatching through a real provider. Swap for a vendor-backed implementation
/// (SMTP/SendGrid/SES/...) once one is chosen; nothing else in the app needs to change.
/// </summary>
public class LoggingEmailService(ILogger<LoggingEmailService> logger) : IEmailService
{
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Email to {To} | Subject: {Subject}\n{Body}",
            message.To, message.Subject, message.Body);

        return Task.CompletedTask;
    }
}
