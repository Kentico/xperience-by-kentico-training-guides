using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Profile;
using TrainingGuides.Web.Features.Shared.Models;

public class RegistrationWidgetViewModel : GuidesMemberProfileViewModel, IWidgetViewModel
{
    //WIDGET DISPLAY PROPERTIES

    /// <summary>
    /// Determines whether the widget is misconfigured.
    /// </summary>
    public bool IsMisconfigured =>
        string.IsNullOrWhiteSpace(BaseUrl)
        || string.IsNullOrWhiteSpace(SubmitButtonText)
        || string.IsNullOrWhiteSpace(UserNameLabel)
        || string.IsNullOrWhiteSpace(EmailAddressLabel)
        || string.IsNullOrWhiteSpace(PasswordLabel)
        || string.IsNullOrWhiteSpace(ConfirmPasswordLabel)
        || (ShowName && (string.IsNullOrWhiteSpace(GivenNameLabel) || string.IsNullOrWhiteSpace(FamilyNameLabel) || string.IsNullOrWhiteSpace(FamilyNameFirstLabel)))
        || (ShowExtraFields && string.IsNullOrWhiteSpace(FavoriteCoffeeLabel));

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
    [RegularExpression("^[a-zA-Z0-9_\\-\\.]+$")]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [DataType(DataType.EmailAddress)]
    [Required()]
    [EmailAddress()]
    [MaxLength(100)]
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
