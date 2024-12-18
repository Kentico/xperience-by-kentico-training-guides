

using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.Registration;
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
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    public const string IDENTIFIER = "TrainingGuides.RegistrationWidget";

    public RegistrationWidgetViewComponent(IHttpRequestService httpRequestService,
        IMembershipService membershipService,
        IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.httpRequestService = httpRequestService;
        this.membershipService = membershipService;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }

    public async Task<RegistrationWidgetViewModel> BuildWidgetViewModel(RegistrationWidgetProperties properties) =>
        new()
        {
            BaseUrl = httpRequestService.GetBaseUrl(),
            Language = preferredLanguageRetriever.Get(),
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


}