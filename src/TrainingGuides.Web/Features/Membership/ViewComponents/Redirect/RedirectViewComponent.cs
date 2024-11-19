using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Membership.ViewComponents.Redirect;
public class RedirectViewComponent : ViewComponent
{
    private readonly IHttpRequestService httpRequestService;
    public RedirectViewComponent(IHttpRequestService httpRequestService)
    {
        this.httpRequestService = httpRequestService;
    }
    public IViewComponentResult Invoke(string redirectUrl)
    {
        httpRequestService.RedirectToUrl(redirectUrl);
        return View("~/Features/Membership/ViewComponents/Redirect/Redirect.cshtml");
    }

}
