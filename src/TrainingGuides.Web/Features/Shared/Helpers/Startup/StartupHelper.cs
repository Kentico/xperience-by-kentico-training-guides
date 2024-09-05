using CMS.DataEngine;
using TrainingGuides.DataProtectionCustomizations;

namespace TrainingGuides.Web.Features.Shared.Helpers.Startup;

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
            //Do not resolve ICookieLevelConsentMappingInfoProvider or ICookieConsentService--
            //This code runs on startup before the app is built, so no ServiceProvider exists.
            var consentQuery = await CookieLevelConsentMappingInfo.Provider.Get()
                .GetEnumerableTypedResultAsync();

            var consent = consentQuery.FirstOrDefault();

            consentCodeName = consent?.MarketingConsentCodeName?.FirstOrDefault() ?? string.Empty;
        }
        catch (DataClassNotFoundException)
        {
            consentCodeName = string.Empty;
        }

        return consentCodeName ?? string.Empty;
    }
}
