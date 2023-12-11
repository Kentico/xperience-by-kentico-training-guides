using TrainingGuides.Admin;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.DataProtection.Services;

namespace TrainingGuides.Web.Features.DataProtection.Controllers;

public class ConsentCodnameController : Controller
{
    private readonly ICookieConsentService cookieConsentService;

    public ConsentCodnameController(ICookieConsentService cookieConsentService)
    {
        this.cookieConsentService = cookieConsentService;
    }

    //Provides the name of the consent mapped to the Marketing cookie level for cross-site tracking.
    [HttpGet("consent/marketing")]
    public async Task<IActionResult> MarketingConsent()
    {
        CookieLevelConsentMappingInfo mapping = await cookieConsentService.GetCurrentMapping();
        string marketingConsent = mapping.MarketingConsentCodeName.FirstOrDefault();
        return Json(marketingConsent);
    }
}
