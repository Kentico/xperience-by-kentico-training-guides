using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using CMS.ContactManagement;
using CMS.DataProtection;
using CMS.Helpers;
using CMS.DataEngine;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Web.Mvc;
using TrainingGuides.DataProtectionCustomizations;
using TrainingGuides.Web.Features.DataProtection.Services;
using TrainingGuides.Web.Features.DataProtection.Shared;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.DataProtection.ViewComponents.TrackingConsent;

public class TrackingConsentViewComponent : ViewComponent
{
    private readonly IConsentAgreementService consentAgreementService;
    private readonly IInfoProvider<ConsentInfo> consentInfoProvider;
    private readonly IStringEncryptionService stringEncryptionService;
    private readonly ICookieAccessor cookieAccessor;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    private readonly ICookieConsentService cookieConsentService;
    private readonly IStringLocalizer<SharedResources> stringLocalizer;
    private readonly IHttpRequestService httpRequestService;


    public TrackingConsentViewComponent(
        IConsentAgreementService consentAgreementService,
        IInfoProvider<ConsentInfo> consentInfoProvider,
        IStringEncryptionService stringEncryptionService,
        ICookieAccessor cookieAccessor,
        IPreferredLanguageRetriever preferredLanguageRetriever,
        ICookieConsentService cookieConsentService,
        IStringLocalizer<SharedResources> stringLocalizer,
        IHttpRequestService httpRequestService)
    {
        this.consentAgreementService = consentAgreementService;
        this.consentInfoProvider = consentInfoProvider;
        this.stringEncryptionService = stringEncryptionService;
        this.cookieAccessor = cookieAccessor;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
        this.cookieConsentService = cookieConsentService;
        this.stringLocalizer = stringLocalizer;
        this.httpRequestService = httpRequestService;
    }

    /// <summary>
    /// Invokes the view component, ensures the correct cookie level based on accepted consents
    /// </summary>
    /// <returns>cookie banner view if visitor has not chosen a cookie level, empty if it has already been chosen </returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var currentMapping = await cookieConsentService.GetCurrentMapping();

        if (currentMapping == null
            || currentMapping.PreferenceConsentCodeName.Count() == 0
            || currentMapping.AnalyticalConsentCodeName.Count() == 0
            || currentMapping.MarketingConsentCodeName.Count() == 0)
        {
            return Content(string.Empty);
        }

        var consents = await consentInfoProvider
            .Get()
            .WhereIn("ConsentName", new string[] {
                currentMapping.PreferenceConsentCodeName.FirstOrDefault(),
                currentMapping.AnalyticalConsentCodeName.FirstOrDefault(),
                currentMapping.MarketingConsentCodeName.FirstOrDefault() })
            .GetEnumerableTypedResultAsync();

        if (consents.Count() > 0)
        {
            ContactInfo currentContact = ContactManagementContext.GetCurrentContact(false);

            string text = "<ul>";
            List<string> codenames = [];
            bool isAgreed = false;
            foreach (var consent in consents)
            {
                codenames.Add(consent.ConsentName);

                text += $"<li>{(await consent.GetConsentTextAsync(preferredLanguageRetriever.Get())).ShortText}</li>";

                //agreed will end up being true if the contact has agreed to at least one consent
                isAgreed = isAgreed || ((currentContact != null) && consentAgreementService.IsAgreed(currentContact, consent));
            }
            text += "</ul>";

            string mapping = codenames.Join(Environment.NewLine);

            // Sets the cookie level according to which consents have been accepted
            // Required for scenarios where one contact uses multiple browsers, in case of revoked consent
            EnsureCorrectCookieLevel(currentContact, consents, currentMapping);

            var consentModel = new TrackingConsentViewModel
            {
                CookieAccepted = ValidationHelper.GetBoolean(cookieAccessor.Get(CookieNames.COOKIE_ACCEPTANCE), false),

                CookieMessage = new HtmlString(text),

                CookieHeader = new HtmlString(stringLocalizer["This site uses cookies in the following ways"]),

                AcceptMessage = stringLocalizer["Accept all cookies"],

                // Checks whether the current contact has given an agreement for a cookie level
                IsAgreed = isAgreed,

                RedirectUrl = "/cookie-policy",

                ConfigureMessage = stringLocalizer["Configure cookies"],

                ConsentMapping = stringEncryptionService.EncryptString(mapping),

                BaseUrl = httpRequestService.GetBaseUrl(),

                BaseUrlWithLanguage = httpRequestService.GetBaseUrlWithLanguage()

            };

            // Displays a view with tracking consent information and actions
            return View("~/Features/DataProtection/ViewComponents/TrackingConsent/TrackingConsent.cshtml", consentModel);
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
    private bool EnsureCorrectCookieLevel(
        ContactInfo contact,
        IEnumerable<ConsentInfo> consents,
        CookieLevelConsentMappingInfo mapping)
    {

        var level = ValidationHelper.GetBoolean(cookieAccessor.Get(CookieNames.COOKIE_ACCEPTANCE), false)
                ? CookieConsentLevel.Essential
                : CookieConsentLevel.NotSet;
        var preferenceConsent = consents
            .Where(consent => consent.ConsentName == mapping.PreferenceConsentCodeName.FirstOrDefault())
            .FirstOrDefault();

        if (contact != null && preferenceConsent != null && consentAgreementService.IsAgreed(contact, preferenceConsent))
        {
            level = CookieConsentLevel.Preference;
            var analyticalConsent = consents
                .Where(consent => consent.ConsentName == mapping.AnalyticalConsentCodeName.FirstOrDefault())
                .FirstOrDefault();

            if (contact != null && analyticalConsent != null && consentAgreementService.IsAgreed(contact, analyticalConsent))
            {
                level = CookieConsentLevel.Analytical;
                var marketingConsent = consents
                    .Where(consent => consent.ConsentName == mapping.MarketingConsentCodeName.FirstOrDefault())
                    .FirstOrDefault();

                if (contact != null && marketingConsent != null && consentAgreementService.IsAgreed(contact, marketingConsent))
                {
                    level = CookieConsentLevel.Marketing;
                }
            }
        }
        return cookieConsentService.UpdateCookieLevels(level);
    }
}