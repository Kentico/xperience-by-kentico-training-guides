using CMS.EmailEngine;
using Microsoft.Extensions.Options;

namespace TrainingGuides.Web.Features.EmailNotifications;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly IEmailService emailService;
    private readonly IOptionsSnapshot<EmailNotificationOptions> emailNotificationOptions;

    public EmailNotificationService(IEmailService emailService, IOptionsSnapshot<EmailNotificationOptions> emailNotificationOptions)
    {
        this.emailService = emailService;
        this.emailNotificationOptions = emailNotificationOptions;
    }

    public async Task SendEmailAsync(string subject, string message)
    {
        var msg = new EmailMessage()
        {
            From = emailNotificationOptions.Value.SenderAddress,

            Recipients = emailNotificationOptions.Value.RecipientAddresses,

            Priority = EmailPriorityEnum.Normal,

            Subject = subject,

            Body = message,
        };
        await emailService.SendEmail(msg);
    }
}