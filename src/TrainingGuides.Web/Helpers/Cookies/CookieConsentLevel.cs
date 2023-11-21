namespace TrainingGuides.Web.Helpers.Cookies;

/// <summary>
/// Cookie consent level types.
/// </summary>
public enum CookieConsentLevel
{
    /// <summary>
    /// Cookie consent level is not set.
    /// </summary>
    NotSet = 0,

    /// <summary>
    /// Only essential cookies which are necessary for running the system.
    /// </summary>
    Essential = 1,

    /// <summary>
    /// Cookies for user preferences.
    /// </summary>
    Preference = 2,

    /// <summary>
    /// Cookies for site usage analysis.
    /// </summary>
    Analytical = 3,

    /// <summary>
    /// All cookies enabling to collect information about visitor.
    /// </summary>
    Marketing = 4
}