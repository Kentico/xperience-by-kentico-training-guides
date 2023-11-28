using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Helpers.Cookies;

namespace TrainingGuides.Web.Features.DataProtection.ViewComponents.CookieAcceptance;

public class CookieAcceptanceViewComponent : ViewComponent
{
    private readonly ICookieAccessor cookieAccessor;
    public CookieAcceptanceViewComponent(ICookieAccessor cookieAccessor)
    {
        this.cookieAccessor = cookieAccessor;
    }

    public IViewComponentResult Invoke()
    {
        // Set acceptance cookie
        cookieAccessor.Set(CookieNames.COOKIE_ACCEPTANCE, "true", new CookieOptions
        {
            Path = null,
            Expires = DateTime.Now.AddYears(1),
            HttpOnly = false,
            SameSite = SameSiteMode.Lax
        });

        return View("~/Features/DataProtection/ViewComponents/CookieAcceptance/CookieAcceptance.cshtml");
    }

}
