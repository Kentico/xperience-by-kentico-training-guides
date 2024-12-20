namespace TrainingGuides.Web.Features.Shared.Services;

public interface IHttpRequestService
{
    /// <summary>
    /// Retrieves Base URL from the current request context.
    /// </summary>
    /// <returns>The base URL. If current request contains language, it will NOT be returned with the base URL.</returns>
    /// <exception cref="NullReferenceException">Thrown when unable to retrieve current request context.</exception>
    string GetBaseUrl();

    /// <summary>
    /// Retrieves Base URL from the current request context. If current site is in a language variant, returns language with the base URL as well
    /// </summary>
    /// <returns>The base URL in current language variant, (e.g. website.com or website.com/es).</returns>
    string GetBaseUrlWithLanguage();

    /// <summary>
    /// Retrieves Base URL from the current request context. If current site is in a language variant, returns language with the base URL as well
    /// </summary>
    /// <param name="checkDatabaseForDefaultLanguage">Determines whether to query the database for the default language when it cannot be determined from route data.</param>
    /// <param name="alwaysIncludeLanguage">Determines whether to always include the language in the URL, even if it is the default language.</param>
    /// <returns>The base URL in current language variant. (e.g. website.com or website.com/es)</returns>
    /// <remarksThis overload exists to avoid adjusting the logic of the original method, which would affect an existing training guide</remarks>
    string GetBaseUrlWithLanguage(bool checkDatabaseForDefaultLanguage, bool alwaysIncludeLanguage = false);

    /// <summary>
    /// Retrieves URL of the currently displayed page for a specific language
    /// </summary>
    /// <param name="language">Two-letter language code (e.g., "es" for Spanish, "en" for English)</param>
    /// <returns>Language specific URL of the current page (e.g. website.com/es/page)</returns>
    Task<string> GetCurrentPageUrlForLanguage(string language);

    /// <summary>
    /// Retrieves URL of the specified page for a specific language
    /// </summary>
    /// <param name="webpageGuid">Guid of the webpage to retrieve a URL of.</param>
    /// <param name="language">Two-letter language code (e.g., "es" for Spanish, "en" for English)</param>
    /// <returns>Language specific URL of the current page (e.g. website.com/es/page)</returns>
    Task<string> GetPageRelativeUrl(Guid webpageGuid, string language);

    /// <summary>
    /// Retrieves the value of the specified query string parameter
    /// </summary>
    /// <param name="parameter">The name of the query string parameter to retrieve</param>
    /// <returns>The value of the specified query string parameter</returns>
    string GetQueryStringValue(string parameter);

    /// <summary>
    /// Combines two URL paths
    /// </summary>
    /// <param name="path1">First path</param>
    /// <param name="path2">Second path</param>
    /// <returns>Combined paths</returns>
    /// <remarks>Works with or without leading and trailing slashes</remarks>
    string CombineUrlPaths(string path1, string path2);
}
