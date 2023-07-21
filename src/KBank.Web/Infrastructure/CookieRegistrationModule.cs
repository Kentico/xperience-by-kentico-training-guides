using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using KBank.Web.Helpers.Cookies;
using KBank.Web.Infrastructure;

[assembly: RegisterModule(typeof(CookieRegistrationModule))]
namespace KBank.Web.Infrastructure
{
    public class CookieRegistrationModule : Module
    {
        public CookieRegistrationModule()
            : base("CookieRegistration")
        {
        }

        protected override void OnInit()
        {
            base.OnInit();

            CookieHelper.RegisterCookie(CookieNames.COOKIE_CONSENT_LEVEL, CookieLevel.System);
            CookieHelper.RegisterCookie(CookieNames.COOKIE_ACCEPTANCE, CookieLevel.System);
        }
    }
}