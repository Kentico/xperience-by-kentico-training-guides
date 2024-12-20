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

    public const string IDENTIFIER = "TrainingGuides.SignInWidget";

    public SignInWidgetViewComponent(
        IHttpRequestService httpRequestService,
        IMembershipService membershipService)
    {
        this.httpRequestService = httpRequestService;
        this.membershipService = membershipService;
    }

    public async Task<IViewComponentResult> InvokeAsync(SignInWidgetProperties properties) =>
        View("~/Features/Membership/Widgets/SignIn/SignInWidget.cshtml", await BuildWidgetViewModel(properties));

    public async Task<SignInWidgetViewModel> BuildWidgetViewModel(SignInWidgetProperties properties) => new SignInWidgetViewModel
    {
        ActionUrl = GetActionUrl(),
        DefaultRedirectPageGuid = properties.DefaultRedirectPage.FirstOrDefault()?.WebPageGuid ?? Guid.Empty,
        DisplayForm = !await membershipService.IsMemberAuthenticated(),
        FormTitle = properties.FormTitle,
        SubmitButtonText = properties.SubmitButtonText,
        UserNameOrEmailLabel = properties.UserNameLabel,
        PasswordLabel = properties.PasswordLabel,
        StaySignedInLabel = properties.StaySignedInLabel
    };



    private string GetActionUrl()
    {
        string baseUrl = httpRequestService.GetBaseUrlWithLanguage(true, true);
        var actionUrl = new UriBuilder(baseUrl);

        string newPath = httpRequestService.CombineUrlPaths(actionUrl.Path, ApplicationConstants.AUTHENTICATE_ACTION_PATH);
        actionUrl.Path = newPath;

        string? returnUrl = GetReturnUrlFromQueryString();

        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            actionUrl.Query = QueryString.Create(ApplicationConstants.RETURN_URL_PARAMETER, returnUrl).ToString();
        }

        return actionUrl.ToString();
    }

    private string? GetReturnUrlFromQueryString()
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