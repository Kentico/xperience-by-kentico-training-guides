namespace KBank.Web.Helpers.Cookies;

/// <summary>
/// Contains names of all custom cookies extending the solution. Each need to be registered in <see cref="CookieRegistrationModule"/>.
/// </summary>
public static class CookieNames
{
    // System cookies
    public const string COOKIE_CONSENT_LEVEL = "kbank.cookieconsentlevel";
    public const string COOKIE_ACCEPTANCE = "kbank.cookielevelselection";
}