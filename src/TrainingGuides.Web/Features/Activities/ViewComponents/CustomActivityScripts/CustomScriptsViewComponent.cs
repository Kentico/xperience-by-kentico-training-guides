using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.DataProtection.Services;

namespace TrainingGuides.Web.Features.Activities.ViewComponents.CustomActivityScripts;

public class CustomScriptsViewComponent : ViewComponent
{
    private readonly ICookieConsentService cookieConsentService;

    public CustomScriptsViewComponent(ICookieConsentService cookieConsentService)
    {
        this.cookieConsentService = cookieConsentService;
    }

    public IViewComponentResult Invoke()
    {
        var model = new ContactTrackingAllowedViewModel()
        {
            ContactTrackingAllowed = cookieConsentService.CurrentContactIsVisitorOrHigher()
        };

        return View("~/Features/Activities/ViewComponents/CustomScripts/CustomScripts.cshtml", model);
    }
}
