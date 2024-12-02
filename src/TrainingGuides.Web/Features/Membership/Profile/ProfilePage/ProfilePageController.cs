using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides;

[assembly: RegisterWebPageRoute(
    contentTypeName: ProfilePage.CONTENT_TYPE_NAME,
    controllerType: typeof(TrainingGuides.Web.Features.Membership.Profile.ProfilePageController))]

namespace TrainingGuides.Web.Features.Membership.Profile;
public class ProfilePageController : Controller
{
    public IActionResult Index() => new TemplateResult();
}