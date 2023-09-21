using KBank.Web.Helpers.Cookies;
namespace KBank.Web.DataProtection
{
    public class CurrentContactIsTrackableService
    {
        public bool CurrentContactIsTrackable()
        {
            return CookieConsentHelper.CurrentContactIsVisitorOrHigher();
        }
    }
}
