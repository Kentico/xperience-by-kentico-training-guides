using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TrainingGuides.Web.Features.DataProtection.Services;
using TrainingGuides.Web.Features.DataProtection.Shared;
using TrainingGuides.Web.Features.DataProtection.Widgets.CookiePreferences;

namespace TrainingGuides.Web.Features.DataProtection.Controllers;

public class CookiesController : Controller
{
    private const string COOKIE_UPDATE_MESSAGE = "~/Features/DataProtection/Shared/CookieUpdateMessage.cshtml";
    private const string COOKIE_UPDATE_MESSAGE_SUCCESS = "Cookie consents have been successfully updated.";
    private const string COOKIE_UPDATE_MESSAGE_FAILURE = "Unable to update cookie consents. Please try again.";

    private readonly IStringEncryptionService stringEncryptionService;
    private readonly ICookieConsentService cookieConsentService;

    public CookiesController(
        IStringEncryptionService stringEncryptionService,
        ICookieConsentService cookieConsentService)
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
            return ErrorView();
        }

        CookieConsentLevel selectedConsentValue;
        if (requestModel.CookieLevelSelected is > 0 and < 5)
        {
            selectedConsentValue = (CookieConsentLevel)requestModel.CookieLevelSelected;
        }
        else
        {
            return ErrorView();
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
            return ErrorView();
        }

        return SuccessView(COOKIE_UPDATE_MESSAGE_SUCCESS);
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
            return ErrorView();
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
            return ErrorView();
        }

        return SuccessView();
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
            throw new Exception("No encrypted string.");
        }

        Dictionary<int, string> consentMapping;

        try
        {
            string mappingDecrypted = stringEncryptionService.DecryptString(mappingEncrypted);
            consentMapping = JsonConvert.DeserializeObject<Dictionary<int, string>>(mappingDecrypted) ?? [];
        }
        catch
        {
            throw new Exception("Dictionary can't be decrypted or deserialized.");
        }

        if (!(consentMapping.ContainsKey((int)CookieConsentLevel.Preference)
            && consentMapping.ContainsKey((int)CookieConsentLevel.Analytical)
            && consentMapping.ContainsKey((int)CookieConsentLevel.Marketing)))
        {
            throw new Exception("Mapping does not contain the required cookie level keys.");
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

    private IActionResult SuccessView(string message = "") =>
        PartialView(COOKIE_UPDATE_MESSAGE, new CookieUpdateMessageViewModel(message));

    private IActionResult ErrorView() =>
        PartialView(COOKIE_UPDATE_MESSAGE, new CookieUpdateMessageViewModel(COOKIE_UPDATE_MESSAGE_FAILURE));

}
