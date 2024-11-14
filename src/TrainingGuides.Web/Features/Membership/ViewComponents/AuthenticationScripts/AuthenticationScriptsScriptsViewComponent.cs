using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.Features.Membership.ViewComponents.AuthenticationScripts;
public class AuthenticationScriptsViewComponent : ViewComponent
{
    public AuthenticationScriptsViewComponent()
    { }
    public IViewComponentResult Invoke() => View("~/Features/Membership/ViewComponents/AuthenticationScripts/AuthenticationScripts.cshtml");
}
