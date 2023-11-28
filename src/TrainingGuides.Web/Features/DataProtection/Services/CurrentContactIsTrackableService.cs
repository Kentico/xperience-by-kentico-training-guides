using TrainingGuides.Web.Helpers.Cookies;

namespace TrainingGuides.Web.Features.DataProtection.Services;
public class CurrentContactIsTrackableService
{
    public bool CurrentContactIsTrackable() => CookieConsentHelper.CurrentContactIsVisitorOrHigher();
}

