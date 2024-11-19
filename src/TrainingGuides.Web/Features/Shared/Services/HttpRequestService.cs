using Kentico.Content.Web.Mvc;
using TrainingGuides.Web.Features.Shared.Helpers;

namespace TrainingGuides.Web.Features.Shared.Services;


public class HttpRequestService : IHttpRequestService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    private const string WEB_PAGE_URL_PATHS = "Kentico.WebPageUrlPaths";

    public HttpRequestService(
        IHttpContextAccessor httpContextAccessor,
        IWebPageDataContextRetriever webPageDataContextRetriever,
        IWebPageUrlRetriever webPageUrlRetriever)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.webPageUrlRetriever = webPageUrlRetriever;
    }
    private string GetBaseUrl(HttpRequest currentRequest)
    {
        string pathBase = currentRequest.PathBase.ToString();
        string baseUrl = $"{currentRequest.Scheme}://{currentRequest.Host}";

        return string.IsNullOrWhiteSpace(pathBase) ? baseUrl : $"{baseUrl}{pathBase}";
    }

    private HttpRequest RetrieveCurrentRequest() => httpContextAccessor?.HttpContext?.Request
            ?? throw new NullReferenceException("Unable to retrieve current request context.");

    /// <summary>
    /// Retrieves Base URL from the current request context.
    /// </summary>
    /// <returns>The base URL. If current request contains language, it will NOT be returned with the base URL.</returns>
    /// <exception cref="NullReferenceException">Thrown when unable to retrieve current request context.</exception>
    public string GetBaseUrl()
    {
        var currentRequest = RetrieveCurrentRequest();
        return GetBaseUrl(currentRequest);
    }

    /// <summary>
    /// Retrieves Base URL from the current request context. If current site is in a language variant, returns language with the base URL as well
    /// </summary>
    /// <returns>The base URL in current language variant. (e.g. website.com or website.com/es)</returns>
    public string GetBaseUrlWithLanguage()
    {
        var currentRequest = RetrieveCurrentRequest();
        string language = (string?)currentRequest.RouteValues[ApplicationConstants.LANGUAGE_KEY] ?? string.Empty;
        var webPageUrlPathList = ((string?)currentRequest.RouteValues[WEB_PAGE_URL_PATHS])?.Split('/').ToList() ?? [];

        bool notPrimaryLanguage = webPageUrlPathList.Contains(language);

        return GetBaseUrl(currentRequest)
            + (notPrimaryLanguage
                ? $"/{language}"
                : string.Empty);
    }

    /// <summary>
    /// Retrieves URL of the currently displayed page for a specific language
    /// </summary>
    /// <param name="language">Two-letter language code (e.g., "es" for Spanish, "en" for English)</param>
    /// <returns>Language specific URL of the current page (e.g. website.com/es/page)</returns>
    public async Task<string> GetCurrentPageUrlForLanguage(string language)
    {
        var currentPage = webPageDataContextRetriever.Retrieve().WebPage;
        var url = await webPageUrlRetriever.Retrieve(currentPage.WebPageItemID, language);
        return url.RelativePath;
    }

    /// <summary>
    /// Retrieves URL of the specified page for a specific language
    /// </summary>
    /// <param name="webpage">Webpage to retrieve a URL of.</param>
    /// <param name="language">Two-letter language code (e.g., "es" for Spanish, "en" for English)</param>
    /// <returns>Language specific URL of the current page (e.g. website.com/es/page)</returns>
    public async Task<string> GetPageRelativeUrl(Guid webpageGuid, string language)
    {
        var url = await webPageUrlRetriever.Retrieve(webpageGuid, language);
        return url.RelativePath;
    }

    public string GetQueryStringValue(string parameter) => httpContextAccessor.HttpContext?.Request.Query[parameter].ToString() ?? string.Empty;
}
