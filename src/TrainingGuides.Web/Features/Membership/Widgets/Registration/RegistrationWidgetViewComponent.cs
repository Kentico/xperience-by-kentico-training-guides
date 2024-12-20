using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.Registration;
using TrainingGuides.Web.Features.Shared.Helpers;
using TrainingGuides.Web.Features.Shared.Services;



[assembly: RegisterWidget(
    identifier: RegistrationWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(RegistrationWidgetViewComponent),
    name: "Registration",
    propertiesType: typeof(RegistrationWidgetProperties),
    Description = "Displays a registration form for members.",
    IconClass = "icon-lines-rectangle-o")]

namespace TrainingGuides.Web.Features.Membership.Widgets.Registration;
public class RegistrationWidgetViewComponent : ViewComponent
{
    private readonly IHttpRequestService httpRequestService;
    private readonly IMembershipService membershipService;
    public const string IDENTIFIER = "TrainingGuides.RegistrationWidget";

    public RegistrationWidgetViewComponent(IHttpRequestService httpRequestService,
        IMembershipService membershipService)
    {
        this.httpRequestService = httpRequestService;
        this.membershipService = membershipService;
    }

    public async Task<RegistrationWidgetViewModel> BuildWidgetViewModel(RegistrationWidgetProperties properties) =>
        new()
        {
            ActionUrl = GetActionUrl(),
            DisplayForm = !await membershipService.IsMemberAuthenticated(),
            ShowName = properties.ShowName,
            ShowExtraFields = properties.ShowExtraFields,
            FormTitle = properties.FormTitle,
            SubmitButtonText = properties.SubmitButtonText,
            UserNameLabel = properties.UserNameLabel,
            EmailAddressLabel = properties.EmailAddressLabel,
            PasswordLabel = properties.PasswordLabel,
            ConfirmPasswordLabel = properties.ConfirmPasswordLabel,
            GivenNameLabel = properties.GivenNameLabel,
            FamilyNameLabel = properties.FamilyNameLabel,
            FamilyNameFirstLabel = properties.FamilyNameFirstLabel,
            FavoriteCoffeeLabel = properties.FavoriteCoffeeLabel
        };

    public async Task<IViewComponentResult> InvokeAsync(RegistrationWidgetProperties properties)
    {
        var registerModel = await BuildWidgetViewModel(properties);

        return View("~/Features/Membership/Widgets/Registration/RegistrationWidget.cshtml", registerModel);
    }

    private string GetActionUrl()
    {
        string baseUrl = httpRequestService.GetBaseUrlWithLanguage(true, true);
        var actionUrl = new UriBuilder(baseUrl);

        string newPath = httpRequestService.CombineUrlPaths(actionUrl.Path, ApplicationConstants.REGISTER_ACTION_PATH);
        actionUrl.Path = newPath;

        return actionUrl.ToString();
    }

}