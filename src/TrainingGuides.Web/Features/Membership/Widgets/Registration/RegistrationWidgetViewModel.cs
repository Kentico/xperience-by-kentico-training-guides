using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Profile;
using TrainingGuides.Web.Features.Shared.Models;

// WHY SO MANY HIDDEN INPUTS?
// This repository uses a more complex approach with hidden fields to enable membership components as page builder widgets and templates.
// This approach offers flexibility for editors to more easily link to these pages, create member-related campaigns with better UX, and configure the design of the components.
// However, it requires extra development. To see simpler examples using standard MVC routed views for membership functionality, check out the following resources.
//   - The Xperience by Kentico Community Portal, available at https://github.com/Kentico/community-portal.
//   - The Dancing Goat sample project, available through the "Kentico.Xperience.Templates" .NET Templates package.

public class RegistrationWidgetViewModel : GuidesMemberProfileViewModel, IWidgetViewModel
{
    //WIDGET DISPLAY PROPERTIES

    /// <summary>
    /// Determines whether the widget is misconfigured.
    /// </summary>
    public bool IsMisconfigured =>
        string.IsNullOrWhiteSpace(ActionUrl)
        || string.IsNullOrWhiteSpace(SubmitButtonText)
        || string.IsNullOrWhiteSpace(UserNameLabel)
        || string.IsNullOrWhiteSpace(EmailAddressLabel)
        || string.IsNullOrWhiteSpace(PasswordLabel)
        || string.IsNullOrWhiteSpace(ConfirmPasswordLabel)
        || (ShowName && (string.IsNullOrWhiteSpace(GivenNameLabel) || string.IsNullOrWhiteSpace(FamilyNameLabel) || string.IsNullOrWhiteSpace(FamilyNameFirstLabel)))
        || (ShowExtraFields && string.IsNullOrWhiteSpace(FavoriteCoffeeLabel));

    /// <summary>
    /// The Action URL of the form.
    /// </summary>
    public string ActionUrl { get; set; } = string.Empty;

    /// <summary>
    /// Form title
    /// </summary>
    public string FormTitle { get; set; } = string.Empty;

    /// <summary>
    /// Determines whether the widget should display the form.
    /// </summary>
    [HiddenInput]
    public bool DisplayForm { get; set; }

    /// <summary>
    /// Determines whether the widget should display name-related fields.
    /// </summary>
    [HiddenInput]
    public bool ShowName { get; set; }

    /// <summary>
    /// Determines whether the widget should display extra fields.
    /// </summary>
    [HiddenInput]
    public bool ShowExtraFields { get; set; }

    /// <summary>
    /// Submit button text
    /// </summary>
    [HiddenInput]
    public string SubmitButtonText { get; set; } = string.Empty;

    /// <summary>
    /// User name label.
    /// </summary>
    [HiddenInput]
    public string UserNameLabel { get; set; } = string.Empty;

    /// <summary>
    /// Email address label.
    /// </summary>
    [HiddenInput]
    public string EmailAddressLabel { get; set; } = string.Empty;

    /// <summary>
    /// Password label.
    /// </summary>
    [HiddenInput]
    public string PasswordLabel { get; set; } = string.Empty;

    /// <summary>
    /// Password label.
    /// </summary>
    [HiddenInput]
    public string ConfirmPasswordLabel { get; set; } = string.Empty;

    /// <summary>
    /// Given name label.
    /// </summary>
    [HiddenInput]
    public string GivenNameLabel { get; set; } = string.Empty;

    /// <summary>
    /// Family name label.
    /// </summary>
    [HiddenInput]
    public string FamilyNameLabel { get; set; } = string.Empty;

    /// <summary>
    /// Label for checkbox that indicates that the family name should display first.
    /// </summary>
    [HiddenInput]
    public string FamilyNameFirstLabel { get; set; } = string.Empty;

    /// <summary>
    /// Favorite coffee label.
    /// </summary>
    [HiddenInput]
    public string FavoriteCoffeeLabel { get; set; } = string.Empty;

    //FORM PROPERTIES

    [DataType(DataType.Text)]
    [Required()]
    [RegularExpression("^[a-zA-Z0-9_\\-\\.]+$", ErrorMessage = "Please enter a valid username.")]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [DataType(DataType.EmailAddress)]
    [Required()]
    [EmailAddress()]
    [MaxLength(254)]
    public string EmailAddress { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Required()]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Required()]
    [MaxLength(100)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

}
