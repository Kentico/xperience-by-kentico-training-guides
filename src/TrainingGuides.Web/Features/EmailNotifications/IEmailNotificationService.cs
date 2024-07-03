namespace TrainingGuides.Web.Features.EmailNotifications;
public interface IEmailNotificationService
{
    Task SendEmailAsync(string subject, string message);
}