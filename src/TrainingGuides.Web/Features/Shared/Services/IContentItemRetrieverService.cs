using CMS.ContentEngine;

namespace TrainingGuides.Web.Features.Shared.Services;

public interface IContentItemRetrieverService<T>
{
    public Task<T?> RetrieveWebPageById(int webPageItemId,
        string contentTypeName,
        Func<IWebPageContentQueryDataContainer, T> resultSelector,
        int depth = 1);

    public Task<T?> RetrieveWebPageByGuid(Guid webPageItemGuid,
        string contentTypeName,
        Func<IWebPageContentQueryDataContainer, T> resultSelector,
        int depth = 1);

    public Task<IEnumerable<T?>> RetrieveWebPageContentItems(string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter,
        Func<IWebPageContentQueryDataContainer, T> resultSelector);

    public Task<T?> RetrieveContentItemByGuid(
        Guid contentItemGuid,
        string? contentTypeName,
        Func<IContentQueryDataContainer, T> resultSelector,
        int depth);

    public Task<IEnumerable<T?>> RetrieveReusableContentItems(
        string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter,
        Func<IContentQueryDataContainer, T> resultSelector);
}

public interface IContentItemRetrieverService
{
    public Task<IWebPageFieldsSource> RetrieveWebPageById(int webPageItemId, string contentTypeName);

    public Task<IWebPageFieldsSource> RetrieveWebPageByGuid(Guid webPageItemGuid, string contentTypeName);
}
