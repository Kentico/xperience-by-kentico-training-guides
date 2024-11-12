

using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
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
    public const string IDENTIFIER = "TrainingGuides.RegistrationWidget";

    public RegistrationWidgetViewComponent(IHttpRequestService httpRequestService)
    {
        this.httpRequestService = httpRequestService;
    }

    public IViewComponentResult Invoke(RegistrationWidgetProperties properties)
    {
        var registerModel = new RegistrationWidgetViewModel
        {
            BaseUrl = httpRequestService.GetBaseUrl(),
            DisplayForm = true, //TODO add service method to check if member is currently signed in/out
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

        return View("~/Features/Membership/Widgets/Registration/RegistrationWidget.cshtml", registerModel);
    }


}