using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Activities.ViewComponents.Shared;
using TrainingGuides.Web.Features.DataProtection.Services;

namespace TrainingGuides.Web.Features.Activities.ViewComponents.TrackingScripts;

public class TrackingScriptsViewComponent : ViewComponent
{
    private readonly ICookieConsentService cookieConsentService;

    public TrackingScriptsViewComponent(ICookieConsentService cookieConsentService)
    {
        this.cookieConsentService = cookieConsentService;
    }

    public IViewComponentResult Invoke()
    {
        var model = new ContactTrackingAllowedViewModel()
        {
            ContactTrackingAllowed = cookieConsentService.CurrentContactIsVisitorOrHigher()
        };

        return View("~/Features/Activities/ViewComponents/TrackingScripts/TrackingScripts.cshtml", model);
    }
}
