using CMS.DataEngine;
using CMS.DataProtection;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using TrainingGuides.DataProtectionCustomizations;
using TrainingGuides.Web.Features.DataProtection.Services;
using TrainingGuides.Web.Features.DataProtection.Shared;
using TrainingGuides.Web.Features.DataProtection.Widgets.CookiePreferences;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWidget(
    identifier: CookiePreferencesWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(CookiePreferencesWidgetViewComponent),
    name: "Cookie preferences",
    propertiesType: typeof(CookiePreferencesWidgetProperties),
    Description = "Displays a cookie preferences.",
    IconClass = "icon-cookie")]

namespace TrainingGuides.Web.Features.DataProtection.Widgets.CookiePreferences;

public class CookiePreferencesWidgetViewComponent : ViewComponent
{
    //make sure the values of the following 3 constants match names in .resx file(s)
    private const string PREFERENCE_COOKIES_HEADER = "Preference cookies";
    private const string ANALYTICAL_COOKIES_HEADER = "Analytical cookies";
    private const string MARKETING_COOKIES_HEADER = "Marketing cookies";
    private readonly HtmlString consentMissingDescriptionHtml = new("Please ensure that a valid consent is mapped to this cookie level in the Data protection application.");

    /// <summary>
    /// Widget identifier.
    /// </summary>
    public const string IDENTIFIER = "TrainingGuides.CookiePreferencesWidget";

    private readonly IInfoProvider<ConsentInfo> consentInfoProvider;
    private readonly IStringEncryptionService stringEncryptionService;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    private readonly ICookieConsentService cookieConsentService;
    private readonly ICookieAccessor cookieAccessor;
    private readonly IHttpRequestService httpRequestService;
    private readonly IStringLocalizer<SharedResources> stringLocalizer;

    /// <summary>
    /// Creates an instance of <see cref="CookiePreferencesWidgetViewComponent"/> class.
    /// </summary>
    public CookiePreferencesWidgetViewComponent(
        IInfoProvider<ConsentInfo> consentInfoProvider,
        IStringEncryptionService stringEncryptionService,
        IPreferredLanguageRetriever preferredLanguageRetriever,
        ICookieConsentService cookieConsentService,
        ICookieAccessor cookieAccessor,
        IHttpRequestService httpRequestService,
        IStringLocalizer<SharedResources> stringLocalizer)
    {
        this.consentInfoProvider = consentInfoProvider;
        this.stringEncryptionService = stringEncryptionService;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
        this.cookieConsentService = cookieConsentService;
        this.cookieAccessor = cookieAccessor;
        this.httpRequestService = httpRequestService;
        this.stringLocalizer = stringLocalizer;
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

        string mapping = GetMappingString(currentMapping);
        string language = preferredLanguageRetriever.Get();

        return View("~/Features/DataProtection/Widgets/CookiePreferences/CookiePreferencesWidget.cshtml", new CookiePreferencesWidgetViewModel
        {
            EssentialHeader = properties.EssentialHeader,
            EssentialDescription = properties.EssentialDescription,

            // alternatively, use preferenceCookiesConsent.ConsentDisplayName property for header.
            // Be advised, this property does not support multiple language versions in Xperience.
            PreferenceHeader = stringLocalizer[PREFERENCE_COOKIES_HEADER],
            PreferenceDescriptionHtml = new HtmlString((await preferenceCookiesConsent.GetConsentTextAsync(language)).FullText) ?? consentMissingDescriptionHtml,

            AnalyticalHeader = stringLocalizer[ANALYTICAL_COOKIES_HEADER],
            AnalyticalDescriptionHtml = new HtmlString((await analyticalCookiesConsent.GetConsentTextAsync(language)).FullText) ?? consentMissingDescriptionHtml,

            MarketingHeader = stringLocalizer[MARKETING_COOKIES_HEADER],
            MarketingDescriptionHtml = new HtmlString((await marketingCookiesConsent.GetConsentTextAsync(language)).FullText) ?? consentMissingDescriptionHtml,

            CookieLevelSelected = CMS.Helpers.ValidationHelper.GetInteger(cookieAccessor.Get(CookieNames.COOKIE_CONSENT_LEVEL), 1),

            ConsentMapping = stringEncryptionService.EncryptString(mapping),

            ButtonText = properties.ButtonText,

            BaseUrl = httpRequestService.GetBaseUrl()
        });
    }

    /// <summary>
    /// Gets a serialized string representation of the cookie level consent mapping
    /// </summary>
    /// <param name="currentMapping">A CookieLevelConsentMappingInfo object</param>
    /// <returns>A JSON serialized sting representation of the mapping</returns>
    private string GetMappingString(CookieLevelConsentMappingInfo? currentMapping)
    {
        var mapping = currentMapping != null
            ? new Dictionary<int, string>
            {
                { (int)CookieConsentLevel.Preference, currentMapping.PreferenceConsentCodeName.FirstOrDefault() ?? string.Empty },
                { (int)CookieConsentLevel.Analytical, currentMapping.AnalyticalConsentCodeName.FirstOrDefault() ?? string.Empty },
                { (int)CookieConsentLevel.Marketing, currentMapping.MarketingConsentCodeName.FirstOrDefault() ?? string.Empty }
            }
            : [];

        return JsonConvert.SerializeObject(mapping).ToString();
    }
}
