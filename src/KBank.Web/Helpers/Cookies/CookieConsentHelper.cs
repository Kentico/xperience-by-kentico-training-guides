using CMS.ContactManagement;
using CMS.Core;
using CMS.DataProtection;
using CMS.Helpers;
using TrainingGuides.Admin;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingGuides.Web.Helpers.Cookies;

/// <summary>
/// Provides functionality for retrieving consents for contact.
/// </summary>
public class CookieConsentHelper
{
    private static readonly ICurrentCookieLevelProvider _cookieLevelProvider = Service.Resolve<ICurrentCookieLevelProvider>();
    private static readonly IConsentAgreementService _consentAgreementService = Service.Resolve<IConsentAgreementService>();
    private static readonly IConsentInfoProvider _consentInfoProvider = Service.Resolve<IConsentInfoProvider>();
    private static readonly ICookieLevelConsentMappingInfoProvider _cookieLevelConsentMappingInfoProvider = Service.Resolve<ICookieLevelConsentMappingInfoProvider>();
    private static readonly ICookieAccessor _cookieAccessor = Service.Resolve<ICookieAccessor>();


    /// <summary>
    /// Sets current cookie consent level, internally sets system CookieLevel and agrees to provided consents.
    /// </summary>
    /// <param name="level">Cookie consent level to set</param>
    /// <param name="acceptAllList">List of all cookie consents included when the visitor accepts all from the cookie banner</param>
    /// <exception cref="Exception">Throws if no cookie level consent mappings are provided</exception>
    /// <returns>true if the cookie level was updated successfully and all consents in list were successfully agreed</returns>
    public static async Task<bool> SetCurrentCookieConsentLevel(CookieConsentLevel level, IEnumerable<string> acceptAllList)
    {
        if (acceptAllList == null || acceptAllList.Count() == 0)
        {
            throw new Exception();
        }
        bool cookiesUpToDate = UpdateCookieLevels(level);

        // Get current contact after changes to the cookie level
        var currentContact = ContactManagementContext.GetCurrentContact();

        return await AcceptAllConsents(currentContact, acceptAllList) && cookiesUpToDate;
    }


    /// <summary>
    /// Sets current cookie consent level, internally sets system CookieLevel and agrees or revokes profiling consent.
    /// </summary>
    /// <param name="level">Cookie consent level to set</param>
    /// <param name="mapping">Mapping of consent to cookie level when the visitor sets a cookie level from the widget</param>
    /// <returns>true if the consents and cookie levels were updated according to the cookie level, false if there was an issue</returns>
    public static async Task<bool> SetCurrentCookieConsentLevel(CookieConsentLevel level, IDictionary<int, string> mapping)
    {
        if (mapping == null || mapping.Count() == 0)
        {
            return false;
        }

        // Get original contact before changes to the cookie level
        var originalContact = ContactManagementContext.GetCurrentContact();

        bool cookiesUpToDate = UpdateCookieLevels(level);

        // Get current contact after changes to the cookie level
        var currentContact = ContactManagementContext.GetCurrentContact();

        return await UpdatePreferredConsents(level, originalContact, currentContact, mapping) && cookiesUpToDate;
    }


    /// <summary>
    /// Updates the visitor's cookie level to the provided value
    /// </summary>
    /// <param name<returns>true if the cookie level is equa="level">the cookie consent level to set for the visitor</param>
    /// <returns>true if cookie levels were updated or alredy up-to-date, false if there is an exception</returns>
    public static bool UpdateCookieLevels(CookieConsentLevel level)
    {
        // Get current cookie level and adjust it only if it has been changed
        var originalLevel = GetCurrentCookieConsentLevel();

        if (originalLevel == level)
        {
            return true;
        }
        try
        {
            // Set system cookie level according consent level
            SynchronizeCookieLevel(level);

            //Set cookie consent level into client's cookies
            _cookieAccessor.Set(CookieNames.COOKIE_CONSENT_LEVEL, ((int)level).ToString(), new CookieOptions
            {
                Path = null,
                Expires = DateTime.Now.AddYears(1),
                HttpOnly = false,
                SameSite = SameSiteMode.Lax
            });
            return true;
        }
        catch
        {
            return false;
        }

    }


    /// <summary>
    /// Agrees and revokes consents according to the preferred cookie level
    /// </summary>
    /// <param name="level">The cookie level</param>
    /// <param name="originalContact">The ContactInfo object form before the cookie level was changed</param>
    /// <param name="currentContact">The ContactInfo object from after the cookie level was changed</param>
    /// <param name="mapping">The cookie level consent mapping</param>
    /// <returns>true if the consents from the mapping can be found, false if not</returns>
    private static async Task<bool> UpdatePreferredConsents(CookieConsentLevel level, ContactInfo originalContact, ContactInfo currentContact, IDictionary<int, string> mapping)
    {
        // Get consents
        var preferenceCookiesConsent = await _consentInfoProvider.GetAsync(mapping[(int)CookieConsentLevel.Preference]);
        var analyticalCookiesConsent = await _consentInfoProvider.GetAsync(mapping[(int)CookieConsentLevel.Analytical]);
        var marketingCookiesConsent = await _consentInfoProvider.GetAsync(mapping[(int)CookieConsentLevel.Marketing]);

        if (preferenceCookiesConsent == null || analyticalCookiesConsent == null || marketingCookiesConsent == null)
        {
            return false;
        }

        // Agree cookie consents
        if (level >= CookieConsentLevel.Preference && currentContact != null)
        {
            if (!_consentAgreementService.IsAgreed(currentContact, preferenceCookiesConsent))
            {
                _consentAgreementService.Agree(currentContact, preferenceCookiesConsent);
            }
        }
        if (level >= CookieConsentLevel.Analytical && currentContact != null)
        {
            if (!_consentAgreementService.IsAgreed(currentContact, analyticalCookiesConsent))
            {
                _consentAgreementService.Agree(currentContact, analyticalCookiesConsent);
            }
        }
        if (level >= CookieConsentLevel.Marketing && currentContact != null)
        {
            if (!_consentAgreementService.IsAgreed(currentContact, marketingCookiesConsent))
            {
                _consentAgreementService.Agree(currentContact, marketingCookiesConsent);
            }
        }

        // Revoke consents
        if (level < CookieConsentLevel.Preference && originalContact != null)
        {
            if (_consentAgreementService.IsAgreed(originalContact, preferenceCookiesConsent))
            {
                _consentAgreementService.Revoke(originalContact, preferenceCookiesConsent);
            }
        }

        if (level < CookieConsentLevel.Analytical && originalContact != null)
        {
            if (_consentAgreementService.IsAgreed(originalContact, analyticalCookiesConsent))
            {
                _consentAgreementService.Revoke(originalContact, analyticalCookiesConsent);
            }
        }

        if (level < CookieConsentLevel.Marketing && originalContact != null)
        {
            if (_consentAgreementService.IsAgreed(originalContact, marketingCookiesConsent))
            {
                _consentAgreementService.Revoke(originalContact, marketingCookiesConsent);
            }
        }

        return true;
    }


    /// <summary>
    /// Accepts all consents in the provided list for the provided contact
    /// </summary>
    /// <param name="contact">The contact who has accepted the consents</param>
    /// <param name="acceptAllList">The consents that have been acceepted</param>
    private static async Task<bool> AcceptAllConsents(ContactInfo contact, IEnumerable<string> acceptAllList)
    {
        bool allConsentsExist = true;

        foreach (string codename in acceptAllList)
        {
            var consent = await _consentInfoProvider.GetAsync(codename);

            if (consent == null)
            {
                allConsentsExist = false;
            }
            else if (!_consentAgreementService.IsAgreed(contact, consent))
            {
                _consentAgreementService.Agree(contact, consent);
            }
        }

        return allConsentsExist;
    }


    /// <summary>
    /// Gets currently set cookie consent level.
    /// </summary>
    /// <returns>Cookie consent level</returns>
    public static CookieConsentLevel GetCurrentCookieConsentLevel()
    {
        Enum.TryParse<CookieConsentLevel>(_cookieAccessor.Get(CookieNames.COOKIE_CONSENT_LEVEL), out var consent);

        return consent;
    }


    /// <summary>
    /// Synchronizes cookie level with consent level.
    /// </summary>
    /// <param name="level">Consent level</param>
    private static void SynchronizeCookieLevel(CookieConsentLevel level)
    {
        switch (level)
        {
            case CookieConsentLevel.NotSet:
                SetCookieLevelIfChanged(_cookieLevelProvider.GetDefaultCookieLevel());
                break;
            case CookieConsentLevel.Essential:
                SetCookieLevelIfChanged(Kentico.Web.Mvc.CookieLevel.Essential.Level);
                break;
            case CookieConsentLevel.Preference:
            case CookieConsentLevel.Analytical:
            case CookieConsentLevel.Marketing:
                SetCookieLevelIfChanged(Kentico.Web.Mvc.CookieLevel.Visitor.Level);
                break;
            default:
                throw new NotSupportedException($"CookieConsentLevel {level} is not supported.");
        }
    }


    /// <summary>
    /// Sets CMSCookieLevel if it is different from the new one.
    /// </summary>
    /// <param name="newLevel">The new cooie level to which the contact should be set</param>
    private static void SetCookieLevelIfChanged(int newLevel)
    {
        var currentCookieLevel = _cookieLevelProvider.GetCurrentCookieLevel();

        if (newLevel != currentCookieLevel)
        {
            _cookieLevelProvider.SetCurrentCookieLevel(newLevel);
        }
    }


    /// <summary>
    /// Gets the current cookie level consent mapping if it exists
    /// </summary>
    /// <returns>A <see cref="CookieLevelConsentMappingInfo"/> object representing the site's current mappings, <see cref="null"/> if no mapping exists.</returns>
    public static async Task<CookieLevelConsentMappingInfo> GetCurrentMapping()
    {
        var currentMapping = await _cookieLevelConsentMappingInfoProvider.Get().GetEnumerableTypedResultAsync();

        return currentMapping.FirstOrDefault();
    }


    /// <summary>
    /// Checks if the current contact's CMSCookieLevel is Visitor or higher
    /// </summary>
    /// <returns>True if CMSCookieLevel is greather than or equal to 200, false otherwise</returns>
    public static bool CurrentContactIsVisitorOrHigher()
    {
        bool isVisitorOrHigher = false;
        string cookieLevelString = _cookieAccessor.Get("CMSCookieLevel");

        if (int.TryParse(cookieLevelString, out int cookieLevel))
        {
            isVisitorOrHigher = (cookieLevel >= 200);
        }

        return isVisitorOrHigher;
    }
}