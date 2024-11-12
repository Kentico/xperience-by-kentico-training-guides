namespace TrainingGuides.Web.Features.Membership.Widgets.Authentication;

public class SignInWidgetViewModel
{
    /// <summary>
    /// The Base URL of the site
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// The Base URL of the site
    /// </summary>
    public bool DisplayForm { get; set; } = true;

    /// <summary>
    /// Form title
    /// </summary>
    public string FormTitle { get; set; } = string.Empty;

    /// <summary>
    /// Submit button text
    /// </summary>
    public string SubmitButtonText { get; set; } = string.Empty;

    /// <summary>
    /// User name or email label.
    /// </summary>
    public string UserNameOrEmailLabel { get; set; } = string.Empty;

    /// <summary>
    /// Password label.
    /// </summary>
    public string PasswordLabel { get; set; } = string.Empty;

    /// <summary>
    /// Stay signed in label.
    /// </summary>
    public string StaySignedInLabel { get; set; } = string.Empty;
}
