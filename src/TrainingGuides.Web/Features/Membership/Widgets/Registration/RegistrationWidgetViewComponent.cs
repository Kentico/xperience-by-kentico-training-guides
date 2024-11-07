

using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Widgets.Registration;



[assembly: RegisterWidget(
    identifier: RegistrationWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(RegistrationWidgetViewComponent),
    name: "Registration",
    propertiesType: typeof(RegistrationWidgetProperties),
    Description = "Displays a registration form for members.",
    IconClass = "icon-cookie")]

namespace TrainingGuides.Web.Features.Membership.Widgets.Registration;
public class RegistrationWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.RegistrationWidget";

    public async Task<IViewComponentResult> InvokeAsync(RegistrationWidgetProperties properties)
    {
        return View(properties);
    }
}