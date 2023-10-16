using KBank.Admin;
using KBank.Web.Helpers.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace KBank.Web.DataProtection
{
    public class ConsentCodnameController : Controller
    {
        //Provides the name of the consent mapped to the Marketing cookie level for cross-site tracking.
        [HttpGet]
        [Route("consent/marketing")]
        public async Task<IActionResult> MarketingConsent()
        {
            CookieLevelConsentMappingInfo mapping = await CookieConsentHelper.GetCurrentMapping();
            var marketingConsent = mapping.MarketingConsentCodeName.FirstOrDefault();
            return Json(marketingConsent);
        }
    }
}
