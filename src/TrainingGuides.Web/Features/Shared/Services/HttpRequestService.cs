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
    public string GetBaseUrl()
    {
        var currentRequest = RetrieveCurrentRequest();
        return GetBaseUrl(currentRequest);
    }
}