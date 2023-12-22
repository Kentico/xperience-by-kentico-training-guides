using TrainingGuides.Admin;
using TrainingGuides.Web.Features.DataProtection.Shared;

namespace TrainingGuides.Web.Features.DataProtection.Services;
public interface ICookieConsentService
{
    bool CurrentContactCanBeTracked();
    CookieConsentLevel GetCurrentCookieConsentLevel();
    Task<CookieLevelConsentMappingInfo> GetCurrentMapping();
    Task<bool> SetCurrentCookieConsentLevel(CookieConsentLevel level, IDictionary<int, string> mapping);
    Task<bool> SetCurrentCookieConsentLevel(CookieConsentLevel level, IEnumerable<string> acceptAllList);
    bool UpdateCookieLevels(CookieConsentLevel level);
}
