

using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Widgets.Registration;



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
    public const string IDENTIFIER = "TrainingGuides.RegistrationWidget";

    public IViewComponentResult Invoke(RegistrationWidgetProperties properties) =>
        View("~/Features/Membership/Widgets/Registration/RegistrationWidget.cshtml", new RegistrationWidgetViewModel
        {
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
        });

}