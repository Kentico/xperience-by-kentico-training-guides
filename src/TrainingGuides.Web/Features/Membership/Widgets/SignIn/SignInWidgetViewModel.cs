using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Membership.Widgets.SignIn;

public class SignInWidgetViewModel : IWidgetViewModel
{
    //WIDGET DISPLAY PROPERTIES

    /// <summary>
    /// The Base URL of the site
    /// </summary>
    [HiddenInput]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// The language of the current request
    /// </summary>
    [HiddenInput]
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// URL of the site to redirect to after successful sing in
    /// </summary>
    [HiddenInput]
    public string RedirectUrl { get; set; } = string.Empty;

    /// <summary>
    /// Determines whether the widget should display the form. E.g., if the user is already authenticated, the form should not be displayed. Instead they should see a sign out button
    /// </summary>
    [HiddenInput]
    public bool DisplayForm { get; set; } = true;

    /// <summary>
    /// Determines whether the authentication was successful. If true, the widget will display an a view component that handles the redirection to home page.
    [HiddenInput]
    public bool AuthenticationSuccessful { get; set; } = false;

    /// <summary>
    /// Form title
    /// </summary>
    [HiddenInput]
    public string FormTitle { get; set; } = string.Empty;

    /// <summary>
    /// Submit button text
    /// </summary>
    [HiddenInput]
    public string SubmitButtonText { get; set; } = string.Empty;

    /// <summary>
    /// User name or email label.
    /// </summary>
    [HiddenInput]
    public string UserNameOrEmailLabel { get; set; } = string.Empty;

    /// <summary>
    /// Password label.
    /// </summary>
    [HiddenInput]
    public string PasswordLabel { get; set; } = string.Empty;

    /// <summary>
    /// Stay signed in label.
    /// </summary>
    [HiddenInput]
    public string StaySignedInLabel { get; set; } = string.Empty;

    //FORM PROPERTIES

    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Please enter your user name or email address.")]
    [MaxLength(100)]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Required()]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    public bool StaySignedIn { get; set; } = false;

    public bool IsMisconfigured => string.IsNullOrWhiteSpace(BaseUrl)
        || string.IsNullOrWhiteSpace(SubmitButtonText)
        || string.IsNullOrWhiteSpace(UserNameOrEmailLabel)
        || string.IsNullOrWhiteSpace(PasswordLabel)
        || string.IsNullOrWhiteSpace(StaySignedInLabel);
}
