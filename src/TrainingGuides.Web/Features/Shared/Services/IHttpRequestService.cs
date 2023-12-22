namespace TrainingGuides.Web.Features.Shared.Services;

public interface IHttpRequestService
{
    public string GetBaseUrl();
    public string GetBaseUrlWithLanguage();
    public Task<string> GetCurrentPageUrlForLanguage(string language);
}
