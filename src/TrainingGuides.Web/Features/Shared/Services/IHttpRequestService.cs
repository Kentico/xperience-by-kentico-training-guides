namespace TrainingGuides.Web.Features.Shared.Services;

public interface IHttpRequestService
{
    public string GetBaseUrl();
    public string GetBaseUrlWithLanguage();
    public Task<string> GetCurrentPageUrlForLanguage(string language);
    public Task<string> GetPageRelativeUrl(Guid webpageGuid, string language);
    public string GetQueryStringValue(string parameter);
}
