using CMS.ContentEngine;

namespace TrainingGuides.Web.Features.Shared.Services;

public interface IContentItemRetrieverService<T>
{
    public Task<T?> RetrieveWebPageById(int webPageItemId,
        string contentTypeName,
        int depth = 1);

    public Task<T?> RetrieveWebPageByGuid(Guid? webPageItemGuid,
        string contentTypeName,
        int depth = 1);

    public Task<IEnumerable<T>> RetrieveWebPageContentItems(string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter);

    public Task<IEnumerable<T>> RetrieveWebPageChildrenByPath(
        string parentPageContentTypeName,
        string path,
        int depth = 1);

    public Task<IEnumerable<T>> RetrieveWebPageChildrenByPath(
        string parentPageContentTypeName,
        string path,
        Action<ContentTypeQueryParameters> contentTypeQueryParameters,
        int depth = 1);

    public Task<T?> RetrieveContentItemByGuid(
        Guid contentItemGuid,
        string contentTypeName,
        int depth = 1);

    public Task<IEnumerable<T>> RetrieveReusableContentItems(
        string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter);
}

public interface IContentItemRetrieverService
{
    public Task<IWebPageFieldsSource?> RetrieveWebPageById(int webPageItemId);

    public Task<IWebPageFieldsSource?> RetrieveWebPageByGuid(Guid webPageItemGuid);

    public Task<IEnumerable<IContentItemFieldsSource>> RetrieveContentItemsBySchema(string schemaName, Action<ContentQueryParameters> contentQueryParameters);
}
