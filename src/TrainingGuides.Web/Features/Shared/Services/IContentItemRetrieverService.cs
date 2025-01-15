using CMS.ContentEngine;
using TrainingGuides.Web.Features.Shared.OptionProviders.OrderBy;

namespace TrainingGuides.Web.Features.Shared.Services;

public interface IContentItemRetrieverService<T>
{
    /// <summary>
    /// Retrieves Web page content item by Id using ContentItemQueryBuilder
    /// </summary>
    /// <param name="webPageItemId">The Id of the Web page content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns>A Web page content item of specified type, with the specified Id</returns>
    Task<T?> RetrieveWebPageById(int webPageItemId,
        string contentTypeName,
        int depth = 1);

    /// <summary>
    /// Retrieves Web page content item by Id using ContentItemQueryBuilder
    /// </summary>
    /// <param name="webPageItemGuid">The Guid of the Web page content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns>A Web page content item of specified type, with the specified Id</returns>
    Task<T?> RetrieveWebPageByGuid(Guid? webPageItemGuid,
        string contentTypeName,
        int depth = 1);

    /// <summary>
    /// Retrieves web page content items using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="queryFilter">A delegate used to configure query for given contentTypeName</param>
    /// <returns>An enumerable set of items</returns>
    Task<IEnumerable<T>> RetrieveWebPageContentItems(string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter);

    /// <summary>
    /// Retrieves child pages of a given web page.
    /// </summary>
    /// <param name="parentPageContentTypeName">Content type of the parent page</param>
    /// <param name="parentPagePath">Path of the parent page</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns></returns>
    Task<IEnumerable<T>> RetrieveWebPageChildrenByPath(
        string parentPageContentTypeName,
        string path,
        int depth = 1);

    /// <summary>
    /// Retrieves child pages of a given web page that are linked to specific content items, specified by list of reference IDs.
    /// </summary>
    /// <param name="parentPageContentTypeName">Content type of the parent page</param>
    /// <param name="parentPagePath">Path of the parent page</param>
    /// <param name="referenceFieldName">The page field name that contains the reference</param>
    /// <param name="referenceIds">Enumerable of IDs of content items</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns></returns>
    Task<IEnumerable<T>> RetrieveWebPageChildrenByPathAndReference(
        string parentPageContentTypeName,
        string parentPagePath,
        string referenceFieldName,
        IEnumerable<int> referenceIds,
        int depth = 1
    );

    /// <summary>
    /// Retrieves Web page content item by Id using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentItemGuid">The Guid of the reusable content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns>A Web page content item of specified type, with the specified Id</returns>
    Task<T?> RetrieveContentItemByGuid(
        Guid contentItemGuid,
        string contentTypeName,
        int depth = 1);

    /// <summary>
    /// Retrieves reusable content items using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentTypeName">Content type name of the reusable item.</param>
    /// <param name="queryFilter">A delegate used to configure query for given contentTypeName</param>
    /// <returns>An enumerable set of items</returns>
    Task<IEnumerable<T>> RetrieveReusableContentItems(
        string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter);

    /// <summary>
    /// Retrieves reusable content items of specified type from specified smart folder.
    /// </summary>
    /// <param name="contentTypeName">Content type name of the content items the method should return</param>
    /// <param name="smartFolderGuid">Guid of the smart folder to retrieve the content items from</param>
    /// <param name="orderBy">Order the returned items ascending/descending</param>
    /// <param name="topN">Number of items to return</param>
    /// <returns></returns>
    Task<IEnumerable<T>> RetrieveReusableContentItemsFromSmartFolder(
        string contentTypeName,
        Guid smartFolderGuid,
        OrderByOption orderBy,
        int topN = 20,
        int depth = 1);
}

public interface IContentItemRetrieverService
{
    public Task<IWebPageFieldsSource?> RetrieveWebPageById(int webPageItemId);

    public Task<IWebPageFieldsSource?> RetrieveWebPageByGuid(Guid webPageItemGuid);

    public Task<IEnumerable<IContentItemFieldsSource>> RetrieveContentItemsBySchemaAndTags(
        string schemaName,
        string taxonomyColumnName,
        IEnumerable<Guid> tagGuids);
}