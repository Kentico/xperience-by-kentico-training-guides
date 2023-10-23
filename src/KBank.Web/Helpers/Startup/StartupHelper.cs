using CMS.DataEngine;
using KBank.Web.Helpers.Cookies;
using System.Linq;
using System.Threading.Tasks;

namespace KBank.Web.Helpers.Startup
{
    public class StartupHelper
    {
        /// <summary>
        /// Retrieves the codename of the currently mapped Marketing consent, returning an empty string if it is not found
        /// </summary>
        /// <remarks>
        /// Used in Program.cs to prevent <see cref="DataClassNotFoundException"/> when running CI restore for the first time.
        /// </remarks>
        /// <returns></returns>
        public static async Task<string> GetMarketingConsentCodeName()
        {
            string consentCodeName;
            try
            {
                var consent = await CookieConsentHelper.GetCurrentMapping();
                consentCodeName = consent.MarketingConsentCodeName.FirstOrDefault();
            }
            catch (DataClassNotFoundException)
            {
                consentCodeName = string.Empty;
            }

            return consentCodeName;
        }
    }
}
