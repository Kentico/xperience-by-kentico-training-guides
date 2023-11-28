using TrainingGuides.Admin;
using TrainingGuides.Web.Helpers.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.Features.DataProtection.Controllers;

public class ConsentCodnameController : Controller
{
    //Provides the name of the consent mapped to the Marketing cookie level for cross-site tracking.
    [HttpGet]
    [Route("consent/marketing")]
    public async Task<IActionResult> MarketingConsent()
    {
        CookieLevelConsentMappingInfo mapping = await CookieConsentHelper.GetCurrentMapping();
        string marketingConsent = mapping.MarketingConsentCodeName.FirstOrDefault();
        return Json(marketingConsent);
    }
}
