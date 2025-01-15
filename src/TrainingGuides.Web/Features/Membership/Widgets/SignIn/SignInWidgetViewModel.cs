using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Membership.Widgets.SignIn;

// WHY SO MANY HIDDEN INPUTS?
// This repository uses a more complex approach with hidden fields to enable membership components as page builder widgets and templates.
// This approach offers flexibility for editors to more easily link to these pages, create member-related campaigns with better UX, and configure the design of the components.
// However, it requires extra development. To see simpler examples using standard MVC routed views for membership functionality, check out the following resources.
//   - The Xperience by Kentico Community Portal, available at https://github.com/Kentico/community-portal.
//   - The Dancing Goat sample project, available through the "Kentico.Xperience.Templates" .NET Templates package.

public class SignInWidgetViewModel : IWidgetViewModel
{
    //WIDGET DISPLAY PROPERTIES

    /// <summary>
    /// The action URL of the sign-in form
    /// </summary>
    public string ActionUrl { get; set; } = string.Empty;

    /// <summary>
    /// URL to redirect to after successful sign in
    /// </summary>
    public string RedirectUrl { get; set; } = string.Empty;

    /// <summary>
    /// URL of the site to redirect to after successful sing in, configured in the widget properties.
    /// </summary>
    [HiddenInput]
    public Guid DefaultRedirectPageGuid { get; set; } = Guid.Empty;

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
    [MaxLength(254)]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Required()]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    public bool StaySignedIn { get; set; } = false;

    public bool IsMisconfigured => string.IsNullOrWhiteSpace(ActionUrl)
        || string.IsNullOrWhiteSpace(SubmitButtonText)
        || string.IsNullOrWhiteSpace(UserNameOrEmailLabel)
        || string.IsNullOrWhiteSpace(PasswordLabel)
        || string.IsNullOrWhiteSpace(StaySignedInLabel);
}
