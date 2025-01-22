namespace TrainingGuides.Web.Features.Shared.Services;

public class HttpRequestService : IHttpRequestService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public HttpRequestService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
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


    /// <summary>
    /// Retrieves Base URL from the current request context.
    /// </summary>
    /// <returns>The base URL. If current request contains language, it will NOT be returned with the base URL.</returns>
    public string GetBaseUrl()
    {
        var currentRequest = RetrieveCurrentRequest();
        return GetBaseUrl(currentRequest);
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
}