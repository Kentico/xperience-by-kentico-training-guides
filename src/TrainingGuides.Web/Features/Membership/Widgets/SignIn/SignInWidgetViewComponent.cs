using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.Authentication;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWidget(
    identifier: SignInWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(SignInWidgetViewComponent),
    name: "SignIn",
    propertiesType: typeof(SignInWidgetProperties),
    Description = "Displays a sign in form for members.",
    IconClass = "icon-user")]

namespace TrainingGuides.Web.Features.Membership.Widgets.Authentication;
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

    public async Task<IViewComponentResult> InvokeAsync(SignInWidgetProperties properties)
    {
        var widgetViewModel = new SignInWidgetViewModel
        {
            BaseUrl = httpRequestService.GetBaseUrl(),
            DisplayForm = await membershipService.IsMemberAuthenticated(),
            FormTitle = properties.FormTitle,
            SubmitButtonText = properties.SubmitButtonText,
            UserNameOrEmailLabel = properties.UserNameLabel,
            PasswordLabel = properties.PasswordLabel,
            StaySignedInLabel = properties.StaySignedInLabel
        };

        return View("~/Features/Membership/Widgets/Authentication/SignInWidget.cshtml", widgetViewModel);
    }


}