using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using KBank.Web.Components.Widgets.CookiePreferences;
using CMS.DataProtection;
using System.Globalization;
using KBank.Web.Helpers.Cookies;
using System.Collections.Generic;
using Newtonsoft.Json;
using KBank.Admin;
using System.Linq;
using System.Threading.Tasks;
using KBank.Web.Services.Cryptography;

[assembly:
    RegisterWidget(CookiePreferencesWidgetViewComponent.Identifier, typeof(CookiePreferencesWidgetViewComponent), "Cookie preferences",
        typeof(CookiePreferencesWidgetProperties), Description = "Displays a cookie preferences.",
        IconClass = "icon-cookie")]

namespace KBank.Web.Components.Widgets.CookiePreferences
{
    public class CookiePreferencesWidgetViewComponent : ViewComponent
    {
        private readonly IConsentInfoProvider _consentInfoProvider;
        private readonly IStringEncryptionService _stringEncryptionService;


        private const string CONSENT_MISSING_HEADER = "CONSENT NOT FOUND";
        private const string CONSENT_MISSING_DESCRIPTION = "Please ensure that a valid consent is mapped to this cookie level in the Data protection application.";


        /// <summary>
        /// Widget identifier.
        /// </summary>
        public const string Identifier = "KBank.CookiePreferences";


        /// <summary>
        /// Creates an instance of <see cref="CookiePreferencesWidgetViewComponent"/> class.
        /// </summary>
        public CookiePreferencesWidgetViewComponent(IConsentInfoProvider consentInfoProvider,
                                                    IStringEncryptionService stringEncryptionService) 
        {
            _consentInfoProvider = consentInfoProvider;
            _stringEncryptionService = stringEncryptionService;
        }

        /// <summary>
        /// Invokes the widget view component
        /// </summary>
        /// <param name="properties">The properties of the widget</param>
        /// <returns>The view for the widget</returns>
        public async Task<ViewViewComponentResult> InvokeAsync(CookiePreferencesWidgetProperties properties)
        {
            var currentMapping = await CookieConsentHelper.GetCurrentMapping();

            // Get consents
            var preferenceCookiesConsent = await _consentInfoProvider.GetAsync(currentMapping?.PreferenceConsentCodeName.FirstOrDefault());
            var analyticalCookiesConsent = await _consentInfoProvider.GetAsync(currentMapping?.AnalyticalConsentCodeName.FirstOrDefault());
            var marketingCookiesConsent = await _consentInfoProvider.GetAsync(currentMapping?.MarketingConsentCodeName.FirstOrDefault());

            var mapping = GetMappingString(currentMapping);

            return View("~/Components/Widgets/CookiePreferences/_CookiePreferencesWidget.cshtml", new CookiePreferencesWidgetViewModel
            {
                EssentialHeader = properties.EssentialHeader,
                EssentialDescription = properties.EssentialDescription,
                PreferenceHeader = preferenceCookiesConsent?.ConsentDisplayName ?? CONSENT_MISSING_HEADER,
                PreferenceDescription = preferenceCookiesConsent?.GetConsentText(CultureInfo.CurrentCulture.Name).FullText ?? CONSENT_MISSING_DESCRIPTION,
                AnalyticalHeader = analyticalCookiesConsent?.ConsentDisplayName ?? CONSENT_MISSING_HEADER,
                AnalyticalDescription = analyticalCookiesConsent?.GetConsentText(CultureInfo.CurrentCulture.Name).FullText ?? CONSENT_MISSING_DESCRIPTION,
                MarketingHeader = marketingCookiesConsent?.ConsentDisplayName ?? CONSENT_MISSING_HEADER,
                MarketingDescription = marketingCookiesConsent?.GetConsentText(CultureInfo.CurrentCulture.Name).FullText ?? CONSENT_MISSING_DESCRIPTION,
                ConsentMapping = _stringEncryptionService.EncryptString(mapping),
                ButtonText = properties.ButtonText
            });
        }

        /// <summary>
        /// Gets a serialized string representation of the cookie level consent mapping
        /// </summary>
        /// <param name="currentMapping">A CookieLevelConsentMappingInfo object</param>
        /// <returns>A JSON serialized sting representation of the mapping</returns>
        private string GetMappingString(CookieLevelConsentMappingInfo currentMapping)
        {
            Dictionary<int, string> mapping = new Dictionary<int, string>();

            mapping.Add((int)CookieConsentLevel.Preference, currentMapping?.PreferenceConsentCodeName.FirstOrDefault() ?? string.Empty);
            mapping.Add((int)CookieConsentLevel.Analytical, currentMapping?.AnalyticalConsentCodeName.FirstOrDefault() ?? string.Empty);
            mapping.Add((int)CookieConsentLevel.Marketing, currentMapping?.MarketingConsentCodeName.FirstOrDefault() ?? string.Empty);

            return JsonConvert.SerializeObject(mapping).ToString();
        }
    }
}