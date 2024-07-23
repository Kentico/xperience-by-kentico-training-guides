using CMS.EmailEngine;
using Microsoft.Extensions.Options;

namespace TrainingGuides.Web.Features.EmailNotifications;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly IEmailService emailService;
    private readonly IOptionsMonitor<EmailNotificationOptions> emailNotificationOptions;
    private readonly IOptionsMonitorCache<EmailNotificationOptions> emailNotificationOptionsCache;

    public EmailNotificationService(IEmailService emailService,
    IOptionsMonitor<EmailNotificationOptions> emailNotificationOptions,
    IOptionsMonitorCache<EmailNotificationOptions> emailNotificationOptionsCache)
    {
        this.emailService = emailService;
        this.emailNotificationOptions = emailNotificationOptions;
        this.emailNotificationOptionsCache = emailNotificationOptionsCache;
    }

    public async Task SendEmailAsync(string subject, string message)
    {
        emailNotificationOptionsCache.Clear();

        var msg = new EmailMessage()
        {
            From = emailNotificationOptions.CurrentValue.SenderAddress,

            Recipients = emailNotificationOptions.CurrentValue.RecipientAddresses,

            Priority = EmailPriorityEnum.Normal,

            Subject = subject,

            Body = message,
        };
        await emailService.SendEmail(msg);
    }
}
