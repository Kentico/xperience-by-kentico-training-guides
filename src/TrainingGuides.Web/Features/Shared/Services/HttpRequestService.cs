using Kentico.Content.Web.Mvc.Routing;

namespace TrainingGuides.Web.Features.Shared.Services;

public class HttpRequestService : IHttpRequestService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    private const string WEB_PAGE_URL_PATHS = "Kentico.WebPageUrlPaths";

    public HttpRequestService(IHttpContextAccessor httpContextAccessor,
    IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }
    /// <summary>
    /// Retrieves Base URL from the current request context.
    /// </summary>
    /// <returns>The base URL in current language variant. (e.g. website.com or website.com/es)</returns>
    public string GetBaseUrl()
    {
        var request = httpContextAccessor?.HttpContext?.Request;
        string pathBase = request.PathBase.ToString();
        var webPageUrlPathList = ((string)request.RouteValues[WEB_PAGE_URL_PATHS])?.Split('/').ToList() ?? [];
        string language = preferredLanguageRetriever.Get();

        bool isPrimaryLanguage = webPageUrlPathList.Contains(language);

        string baseUrl = $"{request.Scheme}://{request.Host}"
            + (isPrimaryLanguage
                ? string.Empty
                : $"/{language}");

        return string.IsNullOrWhiteSpace(pathBase) ? baseUrl : $"{baseUrl}{pathBase}";
    }
}
