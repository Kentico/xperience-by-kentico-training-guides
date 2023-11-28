using CMS.DataProtection;
using TrainingGuides.Admin;
using TrainingGuides.Web.Features.DataProtection.Widgets.CookiePreferences;
using TrainingGuides.Web.Helpers.Cookies;
using TrainingGuides.Web.Services.Cryptography;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Newtonsoft.Json;
using TrainingGuides.Web.Features.DataProtection.Services;

[assembly:
    RegisterWidget(CookiePreferencesWidgetViewComponent.Identifier, typeof(CookiePreferencesWidgetViewComponent), "Cookie preferences",
        typeof(CookiePreferencesWidgetProperties), Description = "Displays a cookie preferences.",
        IconClass = "icon-cookie")]

namespace TrainingGuides.Web.Features.DataProtection.Widgets.CookiePreferences;

public class CookiePreferencesWidgetViewComponent : ViewComponent
{
    private const string CONSENT_MISSING_HEADER = "CONSENT NOT FOUND";
    private const string CONSENT_MISSING_DESCRIPTION = "Please ensure that a valid consent is mapped to this cookie level in the Data protection application.";


    /// <summary>
    /// Widget identifier.
    /// </summary>
    public const string Identifier = "TrainingGuides.CookiePreferences";

    private readonly IConsentInfoProvider consentInfoProvider;
    private readonly IStringEncryptionService stringEncryptionService;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    private readonly ICookieConsentService cookieConsentService;

    /// <summary>
    /// Creates an instance of <see cref="CookiePreferencesWidgetViewComponent"/> class.
    /// </summary>
    public CookiePreferencesWidgetViewComponent(
        IConsentInfoProvider consentInfoProvider,
        IStringEncryptionService stringEncryptionService,
        IPreferredLanguageRetriever preferredLanguageRetriever,
        ICookieConsentService cookieConsentService)
    {
        this.consentInfoProvider = consentInfoProvider;
        this.stringEncryptionService = stringEncryptionService;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
        this.cookieConsentService = cookieConsentService;
    }

    /// <summary>
    /// Invokes the widget view component
    /// </summary>
    /// <param name="properties">The properties of the widget</param>
    /// <returns>The view for the widget</returns>
    public async Task<ViewViewComponentResult> InvokeAsync(CookiePreferencesWidgetProperties properties)
    {
        var currentMapping = await cookieConsentService.GetCurrentMapping();

        // Get consents
        var preferenceCookiesConsent = await consentInfoProvider.GetAsync(currentMapping?.PreferenceConsentCodeName.FirstOrDefault());
        var analyticalCookiesConsent = await consentInfoProvider.GetAsync(currentMapping?.AnalyticalConsentCodeName.FirstOrDefault());
        var marketingCookiesConsent = await consentInfoProvider.GetAsync(currentMapping?.MarketingConsentCodeName.FirstOrDefault());

        var mapping = GetMappingString(currentMapping);

        return View("~/Features/DataProtection/Widgets/CookiePreferences/CookiePreferencesWidget.cshtml", new CookiePreferencesWidgetViewModel
        {
            EssentialHeader = properties.EssentialHeader,
            EssentialDescription = properties.EssentialDescription,

            PreferenceHeader = preferenceCookiesConsent?.ConsentDisplayName ?? CONSENT_MISSING_HEADER,
            PreferenceDescription = (await preferenceCookiesConsent?.GetConsentTextAsync(preferredLanguageRetriever.Get())).FullText ?? CONSENT_MISSING_DESCRIPTION,

            AnalyticalHeader = analyticalCookiesConsent?.ConsentDisplayName ?? CONSENT_MISSING_HEADER,
            AnalyticalDescription = (await analyticalCookiesConsent?.GetConsentTextAsync(preferredLanguageRetriever.Get())).FullText ?? CONSENT_MISSING_DESCRIPTION,

            MarketingHeader = marketingCookiesConsent?.ConsentDisplayName ?? CONSENT_MISSING_HEADER,
            MarketingDescription = (await marketingCookiesConsent?.GetConsentTextAsync(preferredLanguageRetriever.Get())).FullText ?? CONSENT_MISSING_DESCRIPTION,

            ConsentMapping = stringEncryptionService.EncryptString(mapping),

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
        var mapping = new Dictionary<int, string>
        {
            { (int)CookieConsentLevel.Preference, currentMapping?.PreferenceConsentCodeName.FirstOrDefault() ?? string.Empty },
            { (int)CookieConsentLevel.Analytical, currentMapping?.AnalyticalConsentCodeName.FirstOrDefault() ?? string.Empty },
            { (int)CookieConsentLevel.Marketing, currentMapping?.MarketingConsentCodeName.FirstOrDefault() ?? string.Empty }
        };

        return JsonConvert.SerializeObject(mapping).ToString();
    }
}
