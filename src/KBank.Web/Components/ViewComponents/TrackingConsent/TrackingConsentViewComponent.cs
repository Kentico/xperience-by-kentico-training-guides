using CMS.ContactManagement;
using CMS.DataProtection;
using CMS.Helpers;
using KBank.Admin;
using KBank.Web.Helpers.Cookies;
using KBank.Web.Models;
using KBank.Web.Services.Cryptography;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KBank.Web.Components.ViewComponents.TrackingConsent
{
    public class TrackingConsentViewComponent : ViewComponent
    {
        private readonly IConsentAgreementService _consentAgreementService;
        private readonly IConsentInfoProvider _consentInfoProvider;
        private readonly IStringEncryptionService _stringEncryptionService;
        private readonly ICookieAccessor _cookieAccessor;
        private readonly IPreferredLanguageRetriever _preferredLanguageRetriever;


        public TrackingConsentViewComponent(IConsentAgreementService consentAgreementService,
                                            IConsentInfoProvider consentInfoProvider,
                                            IStringEncryptionService stringEncryptionService,
                                            ICookieAccessor cookieAccessor,
                                            IPreferredLanguageRetriever preferredLanguageRetriever)
        {
            _consentAgreementService = consentAgreementService;
            _consentInfoProvider = consentInfoProvider;
            _stringEncryptionService = stringEncryptionService;
            _cookieAccessor = cookieAccessor;
            _preferredLanguageRetriever = preferredLanguageRetriever;
        }

        /// <summary>
        /// Invokes the view component, ensures the correct cookie level based on accepted consents
        /// </summary>
        /// <returns>cookie banner view if visitor has not chosen a cookie level, empty if it has already been chosen </returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentMapping = await CookieConsentHelper.GetCurrentMapping();

            if (currentMapping == null || currentMapping.PreferenceConsentCodeName.Count() == 0 || currentMapping.AnalyticalConsentCodeName.Count() == 0 || currentMapping.MarketingConsentCodeName.Count() == 0)
            {
                return Content(string.Empty);
            }

            var consents = await _consentInfoProvider.Get().WhereIn($"ConsentName", new string[] { currentMapping.PreferenceConsentCodeName.FirstOrDefault(), currentMapping.AnalyticalConsentCodeName.FirstOrDefault(), currentMapping.MarketingConsentCodeName.FirstOrDefault() }).GetEnumerableTypedResultAsync();

            if (consents.Count() > 0)
            {
                ContactInfo currentContact = ContactManagementContext.GetCurrentContact(false);

                string text = "<ul>";
                List<string> codenames = new List<string>();
                bool isAgreed = false;
                foreach (ConsentInfo consent in consents)
                {
                    codenames.Add(consent.ConsentName);

                    text += $"<li>{(await consent.GetConsentTextAsync(_preferredLanguageRetriever.Get())).ShortText}</li>";

                    //agreed will end up being true if the contact has agreed to at least one consent
                    isAgreed = isAgreed || (currentContact != null) && _consentAgreementService.IsAgreed(currentContact, consent);
                }
                text += "</ul>";

                string mapping = codenames.Join(System.Environment.NewLine);

                // Sets the cookie level according to which consents have been accepted
                // Required for scenarios where one contact uses multiple browsers, in case of revoked consent
                EnsureCorrectCookieLevel(currentContact, consents, currentMapping);

                var consentModel = new TrackingConsentViewModel
                {
                    CookieAccepted = ValidationHelper.GetBoolean(_cookieAccessor.Get(CookieNames.COOKIE_ACCEPTANCE), false),

                    CookieMessage = text,

                    CookieHeader = "This site uses cookies in the following ways",

                    AcceptMessage = "Accept all cookies",

                    // Checks whether the current contact has given an agreement for a cookie level
                    IsAgreed = isAgreed,

                    RedirectUrl = "/cookie-policy",

                    ConfigureMessage = "Configure cookies",

                    ConsentMapping = _stringEncryptionService.EncryptString(mapping)

                };

                // Displays a view with tracking consent information and actions
                return View("~/Components/ViewComponents/TrackingConsent/_TrackingConsent.cshtml", consentModel);
            }

            return Content(string.Empty);
        }


        /// <summary>
        /// Makes sure that the cookie level of the current visitor matches the consents they are agreed to. This accounts for contacts who revoke consent in one browser then revisit the site in another.
        /// </summary>
        /// <param name="contact">The contact associated with the visitor</param>
        /// <param name="consents">ConsentInfo objects that are mapped to cookie levels</param>
        /// <param name="mapping">Cookie level consent mapping</param>
        /// <returns>true if cookie levels were updated or alredy up-to-date, false if there is an exception</returns>
        private bool EnsureCorrectCookieLevel(ContactInfo contact, IEnumerable<ConsentInfo> consents, CookieLevelConsentMappingInfo mapping)
        {

            CookieConsentLevel level = ValidationHelper.GetBoolean(_cookieAccessor.Get(CookieNames.COOKIE_ACCEPTANCE), false) ? CookieConsentLevel.Essential : CookieConsentLevel.NotSet;
            ConsentInfo preferenceConsent = consents.Where(consent => consent.ConsentName == mapping.PreferenceConsentCodeName.FirstOrDefault()).FirstOrDefault();
            if (contact != null && preferenceConsent != null && _consentAgreementService.IsAgreed(contact, preferenceConsent))
            {
                level = CookieConsentLevel.Preference;
                ConsentInfo analyticalConsent = consents.Where(consent => consent.ConsentName == mapping.AnalyticalConsentCodeName.FirstOrDefault()).FirstOrDefault();
                if (contact != null && analyticalConsent != null && _consentAgreementService.IsAgreed(contact, analyticalConsent))
                {
                    level = CookieConsentLevel.Analytical;
                    ConsentInfo marketingConsent = consents.Where(consent => consent.ConsentName == mapping.MarketingConsentCodeName.FirstOrDefault()).FirstOrDefault();
                    if (contact != null && marketingConsent != null && _consentAgreementService.IsAgreed(contact, marketingConsent))
                    {
                        level = CookieConsentLevel.Marketing;
                    }
                }
            }
            return CookieConsentHelper.UpdateCookieLevels(level);
        }
    }
}