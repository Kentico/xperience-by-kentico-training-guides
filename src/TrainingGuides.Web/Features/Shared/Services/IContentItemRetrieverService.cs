using CMS.Websites;

namespace TrainingGuides.Web.Features.Shared.Services;

public interface IContentItemRetrieverService<T>
{
    public Task<T> RetrieveWebPageById(int webPageItemId, string contentTypeName, Func<IWebPageContentQueryDataContainer, T> resultSelector, int depth = 1);
}

public interface IContentItemRetrieverService
{
    public Task<IWebPageFieldsSource> RetrieveWebPageById(int webPageItemId, string contentTypeName);
}
