using TrainingGuides.Web.Helpers.Cookies;

namespace TrainingGuides.Web.DataProtection;
public class CurrentContactIsTrackableService
{
    public bool CurrentContactIsTrackable()
    {
        return CookieConsentHelper.CurrentContactIsVisitorOrHigher();
    }
}

