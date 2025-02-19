namespace TrainingGuides.Web.Features.Membership.Widgets.ResetPassword;

public class ResetPasswordSuccessViewModel
{
    public const string LINK_STYLES = "btn tg-btn-secondary text-uppercase my-4";
    public string SignInUrl { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string SuccessText { get; set; } = string.Empty;
    public string SignInLinkText { get; set; } = string.Empty;
    public string HomeLinkText { get; set; } = string.Empty;
}