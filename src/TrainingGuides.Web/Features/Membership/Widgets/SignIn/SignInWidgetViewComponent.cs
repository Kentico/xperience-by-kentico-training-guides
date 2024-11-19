using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.SignIn;
using TrainingGuides.Web.Features.Shared.Helpers;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWidget(
    identifier: SignInWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(SignInWidgetViewComponent),
    name: "SignIn",
    propertiesType: typeof(SignInWidgetProperties),
    Description = "Displays a sign in form for members.",
    IconClass = "icon-user")]

namespace TrainingGuides.Web.Features.Membership.Widgets.SignIn;
public class SignInWidgetViewComponent : ViewComponent
{
    private readonly IHttpRequestService httpRequestService;
    private readonly IMembershipService membershipService;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;

    public const string IDENTIFIER = "TrainingGuides.SignInWidget";

    public SignInWidgetViewComponent(
        IHttpRequestService httpRequestService,
        IMembershipService membershipService,
        IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.httpRequestService = httpRequestService;
        this.membershipService = membershipService;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }

    public async Task<IViewComponentResult> InvokeAsync(SignInWidgetProperties properties) =>
        View("~/Features/Membership/Widgets/SignIn/SignInWidget.cshtml", await BuildWidgetViewModel(properties));

    public async Task<SignInWidgetViewModel> BuildWidgetViewModel(SignInWidgetProperties properties)
    {
        string? returnUrl = GetReturnUrl();

        var redirectPage = properties.RedirectPage.FirstOrDefault();

        string redirectUrl = returnUrl
            ?? (redirectPage == null
                ? "/"
                : (await httpRequestService.GetPageRelativeUrl(redirectPage.WebPageGuid, preferredLanguageRetriever.Get())).Replace("~", ""));

        return new SignInWidgetViewModel
        {
            BaseUrl = httpRequestService.GetBaseUrl(),
            RedirectUrl = redirectUrl,
            DisplayForm = !await membershipService.IsMemberAuthenticated(),
            FormTitle = properties.FormTitle,
            SubmitButtonText = properties.SubmitButtonText,
            UserNameOrEmailLabel = properties.UserNameLabel,
            PasswordLabel = properties.PasswordLabel,
            StaySignedInLabel = properties.StaySignedInLabel
        };
    }

    private string? GetReturnUrl()
    {
        string returnUrl = httpRequestService.GetQueryStringValue(ApplicationConstants.RETURN_URL_PARAMETER);

        // If there is no return URL or it is not a relative URL, return null
        if (string.IsNullOrWhiteSpace(returnUrl) || !returnUrl.StartsWith("/"))
        {
            return null;
        }

        return returnUrl;
    }
}