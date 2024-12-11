using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Membership.Widgets.Registration;

public class RegistrationWidgetProperties : IWidgetProperties
{
    /// <summary>
    /// Determines whether the widget should display the Given name and Family name fields.
    /// </summary>
    [CheckBoxComponent(
        Label = "Show name",
        ExplanationText = "Checkbox that determines if the registration form should display given name and family name fields.",
        Order = 10)]
    public bool ShowName { get; set; } = true;

    /// <summary>
    /// Determines whether the widget should display extra fields.
    /// </summary>
    [CheckBoxComponent(
        Label = "Show extra fields",
        ExplanationText = "Checkbox that determines if the registration form should display extra fields.",
        Order = 20)]
    public bool ShowExtraFields { get; set; } = true;

    /// <summary>
    /// Form title    
    /// </summary>
    [TextInputComponent(
        Label = "Form title",
        ExplanationText = "Title to display above the registration form.",
        Order = 30)]
    public string FormTitle { get; set; } = "Sign up";

    /// <summary>
    /// Submit button text   
    /// </summary>
    [TextInputComponent(
        Label = "Submit button text",
        ExplanationText = "Text for the button that submits the registration form.",
        Order = 40)]
    public string SubmitButtonText { get; set; } = "Submit";

    /// <summary>
    /// User name label.
    /// </summary>
    [TextInputComponent(
        Label = "User name label",
        ExplanationText = "Label for the text box where registrants can input their UserName.",
        Order = 50)]
    public string UserNameLabel { get; set; } = "User name";

    /// <summary>
    /// Email address label.
    /// </summary>
    [TextInputComponent(
        Label = "Email address label",
        ExplanationText = "Label for the text box where registrants can input their email address.",
        Order = 60)]
    public string EmailAddressLabel { get; set; } = "Email address";

    /// <summary>
    /// Password label.
    /// </summary>
    [TextInputComponent(
        Label = "Password label",
        ExplanationText = "Label for the text box where registrants can input their password.",
        Order = 70)]
    public string PasswordLabel { get; set; } = "Password";

    /// <summary>
    /// Password label.
    /// </summary>
    [TextInputComponent(
        Label = "Confirm password label",
        ExplanationText = "Label for the text box where registrants can confirm their password.",
        Order = 80)]
    public string ConfirmPasswordLabel { get; set; } = "Confirm your password";

    /// <summary>
    /// Given name label.
    /// </summary>
    [TextInputComponent(
        Label = "Given name label",
        ExplanationText = "Label for the text box where registrants can input their given name.",
        Order = 90)]
    public string GivenNameLabel { get; set; } = "Given name";

    /// <summary>
    /// Family name label.
    /// </summary>
    [TextInputComponent(
        Label = "Family name label",
        ExplanationText = "Label for the text box where registrants can input their family name.",
        Order = 100)]
    public string FamilyNameLabel { get; set; } = "Family name";

    /// <summary>
    /// Label for checkbox that indicates that the family name should display first.
    /// </summary>
    [TextInputComponent(
        Label = "'Family name first' checkbox label",
        ExplanationText = "Label for the checkbox that indicates whether the family name comes before the given name.",
        Order = 110)]
    public string FamilyNameFirstLabel { get; set; } = "Family name goes first";

    /// <summary>
    /// Favorite coffee label.
    /// </summary>
    [TextInputComponent(
        Label = "Favorite coffee label",
        ExplanationText = "Label for the text box where registrants can input their favorite coffee.",
        Order = 120)]
    public string FavoriteCoffeeLabel { get; set; } = "Favorite coffee";

}
