using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Membership.Widgets.Registration;

public class RegistrationWidgetProperties : IWidgetProperties
{
    /// <summary>
    /// Determines whether the widget should display the Given name and Family name fields.
    /// </summary>
    [CheckboxComponent(
        Label = "Show name",
        Order = 10)]
    public bool ShowName { get; set; } = true;

    /// <summary>
    /// Determines whether the widget should display extra fields.
    /// </summary>
    [CheckboxComponent(
        Label = "Show extra fields",
        Order = 20)]
    public bool ShowExtraFields { get; set; } = true;

    /// <summary>
    /// Form title    
    /// </summary>
    [TextInputComponent(
        Label = "Form title",
        Order = 30)]
    public string FormTitle { get; set; } = "Sign up";

    /// <summary>
    /// User name label.
    /// </summary>
    [TextInputComponent(
        Label = "User name label",
        Order = 40)]
    public string UserNameLabel { get; set; } = "User name";

    /// <summary>
    /// Email address label.
    /// </summary>
    [TextInputComponent(
        Label = "Email address label",
        Order = 50)]
    public string EmailAddressLabel { get; set; } = "Email address";

    /// <summary>
    /// Password label.
    /// </summary>
    [TextInputComponent(
        Label = "Password label",
        Order = 60)]
    public string PasswordLabel { get; set; } = "Password";

    /// <summary>
    /// Password label.
    /// </summary>
    [TextInputComponent(
        Label = "Confirm password label",
        Order = 70)]
    public string ConfirmPasswordLabel { get; set; } = "Confirm your password";

    /// <summary>
    /// Given name label.
    /// </summary>
    [TextInputComponent(
        Label = "Given name label",
        Order = 80)]
    public string GivenNameLabel { get; set; } = "Given name";

    /// <summary>
    /// Family name label.
    /// </summary>
    [TextInputComponent(
        Label = "Family name label",
        Order = 90)]
    public string FamilyNameLabel { get; set; } = "Family name";

    /// <summary>
    /// Label for checkbox that indicates that the family name should display first.
    /// </summary>
    [TextInputComponent(
        PasswordLabel = "'Family name first' checkbox label",
        Order = 100)]
    )]
    public bool FamilyNameFirstLabel { get; set; }

    /// <summary>
    /// Favorite coffee label.
    /// </summary>
    [TextInputComponent(
        Label = "Favorite coffee label",
        Order = 110)]
    public string FavoriteCoffeeLabel { get; set; } = "Favorite coffee";

}
