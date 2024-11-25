using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Widgets.ResetPassword;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWidget(
    identifier: ResetPasswordWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(ResetPasswordWidgetViewComponent),
    name: "Reset password",
    propertiesType: typeof(ResetPasswordWidgetProperties),
    Description = "Allows members to request a reset password link.",
    IconClass = "icon-key")]

namespace TrainingGuides.Web.Features.Membership.Widgets.ResetPassword;
public class ResetPasswordWidgetViewComponent : ViewComponent
{
    private readonly IHttpRequestService httpRequestService;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;

    public const string IDENTIFIER = "TrainingGuides.ResetPasswordWidget";

    public ResetPasswordWidgetViewComponent(IHttpRequestService httpRequestService,
        IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.httpRequestService = httpRequestService;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }

    public IViewComponentResult Invoke(ResetPasswordWidgetProperties properties)
    {
        var resetPasswordModel = BuildWidgetViewModel(properties);
        return View("~/Features/Membership/Widgets/ResetPassword/ResetPasswordWidget.cshtml", resetPasswordModel);
    }

    public ResetPasswordWidgetViewModel BuildWidgetViewModel(ResetPasswordWidgetProperties properties) =>
        new()
        {
            BaseUrlWithLanguage = $"{httpRequestService.GetBaseUrl()}/{preferredLanguageRetriever.Get()}",
            SubmitButtonText = properties.SubmitButtonText,
            EmailAddressLabel = properties.EmailAddressLabel
        };

}
