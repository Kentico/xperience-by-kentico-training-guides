using CMS.ContentEngine;
using CMS.Websites;
using System;
using System.Threading.Tasks;

namespace TrainingGuides.Web.Services.Content;

public interface IContentItemRetrieverService<T>
{
    public Task<T> RetrieveWebPageById(int webPageItemId, string contentTypeName, Func<IWebPageContentQueryDataContainer, T> resultSelector, int depth = 1);
    public Task<T> RetrieveContentItem(string contentTypeName, Func<ContentTypeQueryParameters, ContentTypeQueryParameters> filterQuery, Func<IWebPageContentQueryDataContainer, T> selectResult);
}
