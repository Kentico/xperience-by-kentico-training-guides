namespace TrainingGuides.Web.Features.Shared.Services;

public interface IContentItemRetrieverService<T>
{
    public Task<T> RetrieveWebPageById(int webPageItemId, string contentTypeName, Func<IWebPageContentQueryDataContainer, T> resultSelector, int depth = 1);
}
