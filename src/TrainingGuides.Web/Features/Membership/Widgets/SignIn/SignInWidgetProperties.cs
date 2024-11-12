using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Membership.Widgets.Authentication;

public class SignInWidgetProperties : IWidgetProperties
{
    /// <summary>
    /// Form title    
    /// </summary>
    [TextInputComponent(
        Label = "Form title",
        Order = 10)]
    public string FormTitle { get; set; } = "Sign in";

    /// <summary>
    /// Submit button text   
    /// </summary>
    [TextInputComponent(
        Label = "Submit button text",
        Order = 20)]
    public string SubmitButtonText { get; set; } = "Sign in";

    /// <summary>
    /// User name or email label.
    /// </summary>
    [TextInputComponent(
        Label = "User name or email label",
        Order = 30)]
    public string UserNameLabel { get; set; } = "User name or email";

    /// <summary>
    /// Password label.
    /// </summary>
    [TextInputComponent(
        Label = "Password label",
        Order = 40)]
    public string PasswordLabel { get; set; } = "Password";

    /// <summary>
    /// Stay signed in label.
    /// </summary>
    [TextInputComponent(
        Label = "Stay signed in label",
        Order = 50)]
    public string StaySignedInLabel { get; set; } = "Stay signed in";
}
