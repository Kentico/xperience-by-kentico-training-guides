namespace TrainingGuides.Web.Features.Shared.Extensions;

public static class HttpRequestExtensions
{
    public static string GetBaseUrl(this HttpRequest request)
    {
        string pathBase = request.PathBase.ToString();
        string baseUrl = $"{request.Scheme}://{request.Host}";

        return !string.IsNullOrWhiteSpace(pathBase) ? $"{baseUrl}{pathBase}" : baseUrl;
    }
}
