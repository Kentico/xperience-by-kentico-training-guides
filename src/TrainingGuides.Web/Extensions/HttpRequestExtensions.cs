namespace TrainingGuides.Web.Extensions;

public static class HttpRequestExtensions
{
    public static string GetBaseUrl(this HttpRequest request)
    {
        var pathBase = request.PathBase.ToString();
        var baseUrl = $"{request.Scheme}://{request.Host}";

        return !string.IsNullOrWhiteSpace(pathBase) ? $"{baseUrl}{pathBase}" : baseUrl;
    }
}
