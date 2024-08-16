using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.DataProtection.Services;

namespace TrainingGuides.Web.Features.DataProtection.Controllers;

public class ConsentCodenameController : Controller
{
    private readonly ICookieConsentService cookieConsentService;

    public ConsentCodenameController(ICookieConsentService cookieConsentService)
    {
        this.cookieConsentService = cookieConsentService;
    }

    //Provides the name of the consent mapped to the Marketing cookie level for cross-site tracking.
    [HttpGet("consent/marketing")]
    public async Task<IActionResult> MarketingConsent()
    {
        var cookieLevelConsentMapping = await cookieConsentService.GetCurrentMapping();
        string marketingConsent = cookieLevelConsentMapping.MarketingConsentCodeName.FirstOrDefault() ?? string.Empty;
        return Json(marketingConsent);
    }
}
