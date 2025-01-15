namespace TrainingGuides.Web.Features.Membership.Widgets.Registration;

public class EmailConfirmationViewModel
{
    public string Title { get; set; } = string.Empty;
    public EmailConfirmationState State { get; set; } = EmailConfirmationState.FailureNotYetConfirmed;
    public string Message { get; set; } = string.Empty;
    public string ActionButtonText { get; set; } = string.Empty;
    public string SignInOrRegisterPageUrl { get; set; } = string.Empty;
    public string HomePageButtonText { get; set; } = string.Empty;
    public string HomePageUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
