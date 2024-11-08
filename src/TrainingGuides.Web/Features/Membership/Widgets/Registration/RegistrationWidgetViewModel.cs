

namespace TrainingGuides.Web.Features.Membership.Widgets.Registration;

public class RegistrationWidgetViewModel
{
    /// <summary>
    /// The Base URL of the site
    /// </summary>
    public string BaseUrl { get; set; } = "";

    /// <summary>
    /// Determines whether the widget should display the form.
    /// </summary>
    public bool DisplayForm { get; set; }

    /// <summary>
    /// Determines whether the widget should display name-related fields.
    /// </summary>
    public bool ShowName { get; set; }

    /// <summary>
    /// Determines whether the widget should display extra fields.
    /// </summary>
    public bool ShowExtraFields { get; set; }

    /// <summary>
    /// Form title
    /// </summary>
    public string FormTitle { get; set; } = "";

    /// <summary>
    /// Submit button text
    /// </summary>
    public string SubmitButtonText { get; set; } = "";

    /// <summary>
    /// User name label.
    /// </summary>
    public string UserNameLabel { get; set; } = "";

    /// <summary>
    /// Email address label.
    /// </summary>
    public string EmailAddressLabel { get; set; } = "";

    /// <summary>
    /// Password label.
    /// </summary>
    public string PasswordLabel { get; set; } = "";

    /// <summary>
    /// Password label.
    /// </summary>
    public string ConfirmPasswordLabel { get; set; } = "";

    /// <summary>
    /// Given name label.
    /// </summary>
    public string GivenNameLabel { get; set; } = "";

    /// <summary>
    /// Family name label.
    /// </summary>
    public string FamilyNameLabel { get; set; } = "";

    /// <summary>
    /// Label for checkbox that indicates that the family name should display first.
    /// </summary>
    public string FamilyNameFirstLabel { get; set; } = "";

    /// <summary>
    /// Favorite coffee label.
    /// </summary>
    public string FavoriteCoffeeLabel { get; set; } = "";

}
