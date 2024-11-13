using Kentico.Content.Web.Mvc;

namespace TrainingGuides.Web.Features.Shared.Services;

public interface IHttpRequestService
{
    public string GetBaseUrl();
    public string GetBaseUrlWithLanguage();
    public Task<string> GetCurrentPageUrlForLanguage(string language);
    public Task<string> GetPageUrlForLanguage(RoutedWebPage webpage, string language);
}
