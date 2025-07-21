using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Membership.Widgets.SignIn;

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

    [ContentItemSelectorComponent(
        [
            ArticlePage.CONTENT_TYPE_NAME,
            DownloadsPage.CONTENT_TYPE_NAME,
            EmptyPage.CONTENT_TYPE_NAME,
            LandingPage.CONTENT_TYPE_NAME,
            ProductPage.CONTENT_TYPE_NAME,
            ProfilePage.CONTENT_TYPE_NAME
        ],
        Label = "Default redirect page",
        ExplanationText = "Page to redirect to after successful sign in if no 'returnUrl' parameter is specified in the query string. If empty, falls back to the home page.",
        MaximumItems = 1,
        Order = 60)]
    public IEnumerable<ContentItemReference> DefaultRedirectPage { get; set; } = [];
}
