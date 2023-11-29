using CMS.Websites;

namespace TrainingGuides.Web.Services.Content;

public interface IContentItemRetrieverService<T>
{
    public Task<T> RetrieveWebPageById(int webPageItemId, string contentTypeName, Func<IWebPageContentQueryDataContainer, T> resultSelector, int depth = 1);
}
