using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TrainingGuides.Web.Features.DataProtection.Services;
using TrainingGuides.Web.Features.DataProtection.Shared;
using TrainingGuides.Web.Features.DataProtection.Widgets.CookiePreferences;

namespace TrainingGuides.Web.Features.DataProtection.Controllers;

public class CookiesController : Controller
{
    private const string COOKIE_PREFERENCES_SUCCESS = "~/Features/DataProtection/Widgets/CookiePreferences/CookiePreferencesWidgetComplete.cshtml";
    private const string TRACKING_CONSENT_SUCCESS = "~/Features/DataProtection/ViewComponents/TrackingConsent/TrackingConsentComplete.cshtml";
    private const string FAILURE = "~/Features/DataProtection/CookieLevelSetFailed.cshtml";

    private readonly IStringEncryptionService stringEncryptionService;
    private readonly ICookieConsentService cookieConsentService;

    public CookiesController(IStringEncryptionService stringEncryptionService, ICookieConsentService cookieConsentService)
    {
        this.stringEncryptionService = stringEncryptionService;
        this.cookieConsentService = cookieConsentService;
    }


    [HttpPost("/cookies/submit")]
    public async Task<IActionResult> CookiePreferences(CookiePreferencesViewModel requestModel)
    {
        IDictionary<int, string> mapping;
        try
        {
            mapping = GetDictionaryMapping(requestModel.ConsentMapping);
        }
        catch
        {
            return PartialView(FAILURE, requestModel);
        }

        CookieConsentLevel selectedConsentValue;
        if (requestModel.CookieLevelSelected > 0 && requestModel.CookieLevelSelected < 5)
        {
            selectedConsentValue = (CookieConsentLevel)requestModel.CookieLevelSelected;
        }
        else
        {
            return PartialView(FAILURE, requestModel);
        }

        try
        {
            if (!await cookieConsentService.SetCurrentCookieConsentLevel(selectedConsentValue, mapping))
            {
                throw new Exception();
            }
        }
        catch
        {
            return PartialView(FAILURE, requestModel);
        }

        return PartialView(COOKIE_PREFERENCES_SUCCESS, requestModel);
    }


    [HttpPost("/cookies/cookiebannersubmit")]
    public async Task<IActionResult> CookieBanner(CookiePreferencesViewModel requestModel)
    {
        IEnumerable<string> consents;
        try
        {
            consents = GetConsentsList(requestModel.ConsentMapping);
        }
        catch
        {
            return PartialView(FAILURE);
        }

        try
        {
            if (!await cookieConsentService.SetCurrentCookieConsentLevel(CookieConsentLevel.Marketing, consents))
            {
                throw new Exception();
            }
        }
        catch
        {
            return PartialView(FAILURE);
        }

        return PartialView(TRACKING_CONSENT_SUCCESS);
    }

    /// <summary>
    /// Gets a dictionary of consent codenames and the cookie levels to which they are mapped from an encrypted string
    /// </summary>
    /// <param name="mappingEncrypted">The encrypted string representation of the mapping</param>
    /// <returns>A dictionary of integer cookie levels and consent codename values</returns>
    /// <exception cref="Exception">Throws if there is no encrypted string, or if the dictionary can't be decrypted and deserialized, or if the mapping does not contain the required cookie level keys</exception>
    private IDictionary<int, string> GetDictionaryMapping(string mappingEncrypted)
    {
        if (string.IsNullOrEmpty(mappingEncrypted))
        {
            throw new Exception();
        }

        Dictionary<int, string> consentMapping;

        try
        {
            string mapping = stringEncryptionService.DecryptString(mappingEncrypted);
            consentMapping = JsonConvert.DeserializeObject<Dictionary<int, string>>(mapping);
        }
        catch
        {
            throw new Exception();
        }

        if (!(consentMapping.ContainsKey((int)CookieConsentLevel.Preference) && consentMapping.ContainsKey((int)CookieConsentLevel.Analytical) && consentMapping.ContainsKey((int)CookieConsentLevel.Marketing)))
        {
            throw new Exception();
        }

        return consentMapping;
    }

    /// <summary>
    /// Gets a list of consent codenames from an encrypted string
    /// </summary>
    /// <param name="mappingEncrypted">The encrypted string representation of the consents list</param>
    /// <returns>A list of consent codenames</returns>
    /// <exception cref="Exception">Throws if there is no encrypted string, or if no consents are found from decryption</exception>
    private IEnumerable<string> GetConsentsList(string mappingEncrypted)
    {
        if (string.IsNullOrEmpty(mappingEncrypted))
        {
            throw new Exception();
        }

        string mapping = stringEncryptionService.DecryptString(mappingEncrypted);

        IEnumerable<string> consents = mapping.Split(Environment.NewLine).ToList();

        if (consents.Count() == 0)
        {
            throw new Exception();
        }

        return consents;
    }
}
