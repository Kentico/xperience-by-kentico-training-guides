using CMS.ContentEngine;
using CMS.DataEngine;
using Kentico.Content.Web.Mvc;
using TrainingGuides.Web.Features.Shared.Helpers;

namespace TrainingGuides.Web.Features.Shared.Services;

public class HttpRequestService : IHttpRequestService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider;
    private const string WEB_PAGE_URL_PATHS = "Kentico.WebPageUrlPaths";

    public HttpRequestService(
        IHttpContextAccessor httpContextAccessor,
        IWebPageDataContextRetriever webPageDataContextRetriever,
        IWebPageUrlRetriever webPageUrlRetriever,
        IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.webPageUrlRetriever = webPageUrlRetriever;
        this.contentLanguageInfoProvider = contentLanguageInfoProvider;
    }

    private string GetBaseUrl(HttpRequest currentRequest)
    {
        var url = new UriBuilder()
        {
            Scheme = currentRequest.Scheme,
            Host = currentRequest.Host.Host,
            Path = currentRequest.PathBase,
            Port = currentRequest.Host.Port ?? 80
        };

        return url.ToString().TrimEnd('/');
    }

    private HttpRequest RetrieveCurrentRequest() => httpContextAccessor?.HttpContext?.Request
            ?? throw new NullReferenceException("Unable to retrieve current request context.");

    private bool IsLanguageDefault(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return true;

        // Cache this query in real-world scenarios
        string defaultLanguage = contentLanguageInfoProvider.Get()
            .WhereEquals(nameof(ContentLanguageInfo.ContentLanguageIsDefault), true)
            .Column(nameof(ContentLanguageInfo.ContentLanguageName))
            .TopN(1)
            .GetScalarResult<string>();

        if (string.IsNullOrEmpty(defaultLanguage))
            return true;

        return defaultLanguage == language;
    }

    /// <inheritdoc/>
    public string GetBaseUrl()
    {
        var currentRequest = RetrieveCurrentRequest();
        return GetBaseUrl(currentRequest);
    }

    /// <inheritdoc/>
    /// <remarks>When Kentico.WebPageUrlPaths is missing from route values, this method cannot correctly determine whether or not the current language is default. When operating outside of CTB routed pages, use the <see cref="GetBaseUrlWithLanguage(bool,bool)"/> overload instead.</remarks>
    public string GetBaseUrlWithLanguage()
    {
        var currentRequest = RetrieveCurrentRequest();
        string language = (string?)currentRequest.RouteValues[ApplicationConstants.LANGUAGE_KEY] ?? string.Empty;
        var webPageUrlPathList = ((string?)currentRequest.RouteValues[WEB_PAGE_URL_PATHS])?.Split('/').ToList() ?? [];

        bool notPrimaryLanguage = webPageUrlPathList.Contains(language);

        var url = new UriBuilder(GetBaseUrl(currentRequest))
        {
            Path = notPrimaryLanguage
                ? $"/{language}"
                : string.Empty
        };

        return url.ToString();
    }

    /// <inheritdoc/>
    public string GetBaseUrlWithLanguage(bool checkDatabaseForDefaultLanguage, bool alwaysIncludeLanguage = false)
    {
        var currentRequest = RetrieveCurrentRequest();
        string language = (string?)currentRequest.RouteValues[ApplicationConstants.LANGUAGE_KEY] ?? string.Empty;
        var webPageUrlPathList = ((string?)currentRequest.RouteValues[WEB_PAGE_URL_PATHS])?.Split('/').ToList();

        bool notPrimaryLanguage;

        if (checkDatabaseForDefaultLanguage)
        {
            notPrimaryLanguage = webPageUrlPathList?.Contains(language) ?? !IsLanguageDefault(language);
        }
        else
        {
            var newWebPageUrlPathList = webPageUrlPathList ?? [];
            notPrimaryLanguage = newWebPageUrlPathList.Contains(language);
        }
        var url = new UriBuilder(GetBaseUrl(currentRequest))
        {
            Path = notPrimaryLanguage || alwaysIncludeLanguage
                ? $"/{language}"
                : string.Empty
        };

        return url.ToString();
    }

    /// <inheritdoc/>
    public async Task<string> GetCurrentPageUrlForLanguage(string language)
    {
        var currentPage = webPageDataContextRetriever.Retrieve().WebPage;
        var url = await webPageUrlRetriever.Retrieve(currentPage.WebPageItemID, language);
        return url.RelativePath;
    }

    /// <inheritdoc/>
    public async Task<string> GetPageRelativeUrl(Guid webpageGuid, string language)
    {
        var url = await webPageUrlRetriever.Retrieve(webpageGuid, language);
        return url.RelativePath;
    }

    /// <inheritdoc/>
    public string GetQueryStringValue(string parameter) => httpContextAccessor.HttpContext?.Request.Query[parameter].ToString() ?? string.Empty;

    /// <inheritdoc/>
    public string GetAbsoluteUrlForPath(string path, bool alwaysIncludeLanguage, QueryString? queryString = null)
    {
        string trimmedPath = path.TrimStart('~');

        string baseUrl = StartsWithLanguage(trimmedPath)
            ? GetBaseUrl()
            : GetBaseUrlWithLanguage(true, alwaysIncludeLanguage);
        var fullUrl = new UriBuilder(baseUrl);

        string newPath = CombineUrlPaths(fullUrl.Path, trimmedPath);
        fullUrl.Path = newPath;

        if (queryString is not null)
        {
            fullUrl.Query = queryString.ToString();
        }

        return fullUrl.ToString();
    }

    private bool StartsWithLanguage(string relativePath)
    {
        // Cache this query in real-world scenarios
        var languageCodes = contentLanguageInfoProvider.Get()
            .Column(nameof(ContentLanguageInfo.ContentLanguageName))
            .GetListResult<string>();

        string firstPathSegment = relativePath.TrimStart('/').Split('/')[0];

        return languageCodes.Any(code => code.Equals(firstPathSegment, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc/>
    public string CombineUrlPaths(params string[] paths)
    {
        if (paths.Count() == 0)
        {
            return string.Empty;
        }

        var fixedPaths = paths.Select(p => p.Trim('/'));

        return string.Join("/", fixedPaths.Where(p => !string.IsNullOrWhiteSpace(p)));
    }

    /// <inheritdoc/>
    public string ExtractRelativePath(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        var uri = new Uri(url, UriKind.RelativeOrAbsolute);

        return uri.IsAbsoluteUri
            ? uri.PathAndQuery
            : uri.OriginalString;
    }
}
