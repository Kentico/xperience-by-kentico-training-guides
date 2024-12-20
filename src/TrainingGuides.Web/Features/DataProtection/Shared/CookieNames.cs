﻿namespace TrainingGuides.Web.Features.DataProtection.Shared;

/// <summary>
/// Contains names of all custom cookies extending the solution. Each need to be registered in <see cref="CookieRegistrationModule"/>.
/// </summary>
public static class CookieNames
{
    // System cookies
    public const string COOKIE_CONSENT_LEVEL = "trainingguides.cookieconsentlevel";
    public const string COOKIE_ACCEPTANCE = "trainingguides.cookielevelselection";

    // Essential cookies
    public const string COOKIE_AUTHENTICATION = "trainingguides.authentication";

    // Kentico cookies
    public const string CMS_COOKIE_LEVEL = "CMSCookieLevel";
    public const string CURRENT_CONTACT = "CurrentContact";
}
