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
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A Web page content item of specified type, with the specified Id</returns>
    Task<T?> RetrieveWebPageById(int webPageItemId,
        string contentTypeName,
        int depth = 1,
        string? languageName = null);

    /// <summary>
    /// Retrieves Web page content item by Id using ContentItemQueryBuilder
    /// </summary>
    /// <param name="webPageItemGuid">The Guid of the Web page content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A Web page content item of specified type, with the specified web page item Guid</returns>
    Task<T?> RetrieveWebPageByGuid(Guid? webPageItemGuid,
        string contentTypeName,
        int depth = 1,
        string? languageName = null);

    /// <summary>
    /// Retrieves Web page content item by ContentItemGuid using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentItemGuid">The content item Guid of the Web page content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="language">The language of the content item. If null, the language will be inferred from the URL of the current request.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A web page content item of the specified type, with the specified content item Guid</returns>
    Task<T?> RetrieveWebPageByContentItemGuid(
        Guid contentItemGuid,
        string contentTypeName,
        int depth = 1,
        string? languageName = null);

    /// <summary>
    /// Retrieves child pages of a given web page.
    /// </summary>
    /// <param name="parentPageContentTypeName">Content type of the parent page</param>
    /// <param name="parentPagePath">Path of the parent page</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A collection of web pages that exist under the specified path in the content tree</returns>
    Task<IEnumerable<T>> RetrieveWebPageChildrenByPath(
        string parentPageContentTypeName,
        string path,
        bool includeSecuredItems = true,
        int depth = 1,
        string? languageName = null);

    /// <summary>
    /// Retrieves child pages of a given web page that are linked to specific content items, specified by list of reference IDs.
    /// </summary>
    /// <param name="parentPageContentTypeName">Content type of the parent page</param>
    /// <param name="parentPagePath">Path of the parent page</param>
    /// <param name="referenceFieldName">The page field name that contains the reference</param>
    /// <param name="referenceIds">Enumerable of IDs of content items</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A collection of web pages which reference a given collection of content items</returns>
    Task<IEnumerable<T>> RetrieveWebPageChildrenByPathAndReference(
        string parentPageContentTypeName,
        string parentPagePath,
        string referenceFieldName,
        IEnumerable<int> referenceIds,
        bool includeSecuredItems = true,
        int depth = 1,
        string? languageName = null);

    /// <summary>
    /// Retrieves Web page content item by Id using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentItemGuid">The Guid of the reusable content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A Web page content item of specified type, with the specified Id</returns>
    Task<T?> RetrieveContentItemByGuid(
        Guid contentItemGuid,
        string contentTypeName,
        int depth = 1,
        string? languageName = null);

    /// <summary>
    /// Retrieves reusable content items of specified type from specified smart folder.
    /// </summary>
    /// <param name="contentTypeName">Content type name of the content items the method should return</param>
    /// <param name="smartFolderGuid">Guid of the smart folder to retrieve the content items from</param>
    /// <param name="orderBy">Order the returned items ascending/descending</param>
    /// <param name="topN">Number of items to return</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>Enumerable collection of content items from the given smart folder</returns>
    Task<IEnumerable<T>> RetrieveReusableContentItemsFromSmartFolder(
        string contentTypeName,
        Guid smartFolderGuid,
        OrderByOption orderBy,
        int topN = 20,
        int depth = 1,
        string? languageName = null);
}

public interface IContentItemRetrieverService
{
    /// <summary>
    /// Retrieves the IWebPageFieldsSource of a web page item by Id.
    /// </summary>
    /// <param name="webPageItemId">The Id of the web page item</param>
    /// <returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    Task<IWebPageFieldsSource?> RetrieveWebPageById(int webPageItemId);

    /// <summary>
    /// Retrieves the IWebPageFieldsSource of a web page item by Guid.
    /// </summary>
    /// <param name="webPageItemGuid">the Guid of the web page item</param>
    /// <returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    Task<IWebPageFieldsSource?> RetrieveWebPageByContentItemGuid(Guid pageContentItemGuid);

    /// <summary>
    /// Retrieves content items based on the provided schema name and tag guids.
    /// </summary>
    /// <param name="schemaName">The name of the reusable field schema</param>
    /// <param name="taxonomyColumnName">The name of the column that holds the taxonomy value</param>
    /// <param name="tagGuids">Guids of tags to filter the output by</param>
    /// <returns>Enumerable list of content items</returns>
    Task<IEnumerable<IContentItemFieldsSource>> RetrieveContentItemsBySchemaAndTags(
        string schemaName,
        string taxonomyColumnName,
        IEnumerable<Guid> tagGuids);

    /// <summary>
    /// Retrieves the IWebPageFieldsSource of a web page item by path.
    /// </summary>
    /// <param name="pathToMatch">The Tree path of the web page item (can be found in the administration under the Properties tab).</param>
    ///<returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    Task<IWebPageFieldsSource?> RetrieveWebPageByPath(string pathToMatch,
        bool includeSecuredItems = true);
}