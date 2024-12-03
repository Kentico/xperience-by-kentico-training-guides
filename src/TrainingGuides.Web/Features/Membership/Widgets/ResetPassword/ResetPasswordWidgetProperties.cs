using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Membership.Widgets.ResetPassword;

public class ResetPasswordWidgetProperties : IWidgetProperties
{

    /// <summary>
    /// Email address label.
    /// </summary>
    [TextInputComponent(
        Label = "Email address label",
        ExplanationText = "Label for the text box where members can input their email address.",
        Order = 10)]
    public string EmailAddressLabel { get; set; } = "Email address";

    /// <summary>
    /// Submit button text   
    /// </summary>
    [TextInputComponent(
        Label = "Submit button text",
        ExplanationText = "Text for the button that submits the reset password form.",
        Order = 20)]
    public string SubmitButtonText { get; set; } = "Submit";
}