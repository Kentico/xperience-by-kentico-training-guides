namespace TrainingGuides.Web.Features.EmailNotifications;
public class EmailNotificationOptions
{
    public string SenderAddress { get; set; } = string.Empty;

    public string RecipientAddresses { get; set; } = string.Empty;
}