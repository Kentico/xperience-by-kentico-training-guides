using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Activities.ViewComponents.Shared;
using TrainingGuides.Web.Features.DataProtection.Services;

namespace TrainingGuides.Web.Features.Activities.ViewComponents.CustomActivityScripts;

public class CustomActivityScriptsViewComponent : ViewComponent
{
    private readonly ICookieConsentService cookieConsentService;

    public CustomActivityScriptsViewComponent(ICookieConsentService cookieConsentService)
    {
        this.cookieConsentService = cookieConsentService;
    }

    public IViewComponentResult Invoke()
    {
        var model = new ContactTrackingAllowedViewModel()
        {
            ContactTrackingAllowed = cookieConsentService.CurrentContactCanBeTracked()
        };

        return View("~/Features/Activities/ViewComponents/CustomActivityScripts/CustomActivityScripts.cshtml", model);
    }
}
