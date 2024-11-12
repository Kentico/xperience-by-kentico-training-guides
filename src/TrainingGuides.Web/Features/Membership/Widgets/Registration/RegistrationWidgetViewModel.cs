using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
// using TrainingGuides.Web.Features.Membership.Widgets.Registration;

public class RegistrationWidgetViewModel
{
    //WIDGET DISPLAY PROPERTIES

    /// <summary>
    /// The Base URL of the site
    /// </summary>
    [HiddenInput]
    public string BaseUrl { get; set; } = "";

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
    public string FormTitle { get; set; } = "";

    /// <summary>
    /// Submit button text
    /// </summary>
    [HiddenInput]
    public string SubmitButtonText { get; set; } = "";

    /// <summary>
    /// User name label.
    /// </summary>
    [HiddenInput]
    public string UserNameLabel { get; set; } = "";

    /// <summary>
    /// Email address label.
    /// </summary>
    [HiddenInput]
    public string EmailAddressLabel { get; set; } = "";

    /// <summary>
    /// Password label.
    /// </summary>
    [HiddenInput]
    public string PasswordLabel { get; set; } = "";

    /// <summary>
    /// Password label.
    /// </summary>
    [HiddenInput]
    public string ConfirmPasswordLabel { get; set; } = "";

    /// <summary>
    /// Given name label.
    /// </summary>
    [HiddenInput]
    public string GivenNameLabel { get; set; } = "";

    /// <summary>
    /// Family name label.
    /// </summary>
    [HiddenInput]
    public string FamilyNameLabel { get; set; } = "";

    /// <summary>
    /// Label for checkbox that indicates that the family name should display first.
    /// </summary>
    [HiddenInput]
    public string FamilyNameFirstLabel { get; set; } = "";

    /// <summary>
    /// Favorite coffee label.
    /// </summary>
    [HiddenInput]
    public string FavoriteCoffeeLabel { get; set; } = "";

    //FORM PROPERTIES

    [DataType(DataType.Text)]
    [Required()]
    [RegularExpression("^[a-zA-Z0-9_\\-\\.]+$")]
    [MaxLength(100)]
    public string UserName { get; set; } = "";

    [DataType(DataType.EmailAddress)]
    [Required()]
    [EmailAddress()]
    [MaxLength(100)]
    public string EmailAddress { get; set; } = "";

    [DataType(DataType.Password)]
    [Required()]
    [MaxLength(100)]
    public string Password { get; set; } = "";

    [DataType(DataType.Password)]
    [Required()]
    [MaxLength(100)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = "";

    [DataType(DataType.Text)]
    [MaxLength(100)]
    public string GivenName { get; set; } = "";

    [DataType(DataType.Text)]
    [MaxLength(100)]
    public string FamilyName { get; set; } = "";

    public bool FamilyNameFirst { get; set; } = false;

    [DataType(DataType.Text)]
    [MaxLength(100)]
    public string FavoriteCoffee { get; set; } = "";
}
