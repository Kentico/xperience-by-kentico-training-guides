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
        string pathBase = currentRequest.PathBase.ToString();
        string baseUrl = $"{currentRequest.Scheme}://{currentRequest.Host}";

        return string.IsNullOrWhiteSpace(pathBase) ? baseUrl : $"{baseUrl}{pathBase}";
    }

    private HttpRequest RetrieveCurrentRequest() => httpContextAccessor?.HttpContext?.Request
            ?? throw new NullReferenceException("Unable to retrieve current request context.");

    private bool IsLanguageDefault(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return true;

        var defaultLanguage = contentLanguageInfoProvider.Get()
            .WhereEquals(nameof(ContentLanguageInfo.ContentLanguageIsDefault), true)
            .FirstOrDefault();

        if (defaultLanguage == null)
            return true;

        return defaultLanguage.ContentLanguageName == language;
    }

    /// <inheritdoc/>
    public string GetBaseUrl()
    {
        var currentRequest = RetrieveCurrentRequest();
        return GetBaseUrl(currentRequest);
    }

    /// <inheritdoc/>
    /// <remarks>When Kentico.WebPageUrlPaths is missing from route values, this method cannot determine the default language and falls back to default.
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

    /// <inheritdoc/>
    public string GetBaseUrlWithLanguage(bool checkDatabaseForDefaultLanguage)
    {
        if (checkDatabaseForDefaultLanguage)
        {
            var currentRequest = RetrieveCurrentRequest();
            string language = (string?)currentRequest.RouteValues[ApplicationConstants.LANGUAGE_KEY] ?? string.Empty;
            var webPageUrlPathList = ((string?)currentRequest.RouteValues[WEB_PAGE_URL_PATHS])?.Split('/').ToList();

            bool notPrimaryLanguage = webPageUrlPathList?.Contains(language) ?? !IsLanguageDefault(language);

            return GetBaseUrl(currentRequest)
                + (notPrimaryLanguage
                    ? $"/{language}"
                    : string.Empty);
        }
        else
        {
            return GetBaseUrlWithLanguage();
        }
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
}
