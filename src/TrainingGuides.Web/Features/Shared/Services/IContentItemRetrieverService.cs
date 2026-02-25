using CMS.ContentEngine;
using Kentico.Content.Web.Mvc;
using TrainingGuides.Web.Features.Shared.OptionProviders.OrderBy;

namespace TrainingGuides.Web.Features.Shared.Services;

public interface IContentItemRetrieverService
{
    /// <summary>
    /// Retrieves the current page using ContentRetriever API
    /// </summary>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>The current web page content item of the specified type</returns>
    Task<T?> RetrieveCurrentPage<T>(
        int depth = 1,
        bool includeSecuredItems = true,
        string? languageName = null)
        where T : IWebPageFieldsSource, new();

    /// <summary>
    /// Retrieves Web page content item by ContentItemGuid using ContentRetriever API
    /// </summary>
    /// <param name="contentItemGuid">The content item GUID of the Web page content item.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A web page content item of the specified type, with the specified content item Guid</returns>
    Task<T?> RetrieveWebPageByContentItemGuid<T>(
        Guid contentItemGuid,
        int depth = 1,
        bool includeSecuredItems = true,
        string? languageName = null)
        where T : IWebPageFieldsSource, new();

    /// <summary>
    /// Retrieves child pages of a given web page using ContentRetriever API
    /// </summary>
    /// <param name="path">Path of the parent page</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A collection of web pages that exist under the specified path in the content tree</returns>
    Task<IEnumerable<T>> RetrieveWebPageChildrenByPath<T>(
        string path,
        int depth = 1,
        bool includeSecuredItems = true,
        string? languageName = null)
        where T : IWebPageFieldsSource, new();

    /// <summary>
    /// Retrieves child pages of a given web page using ContentRetriever API with additional query configuration
    /// </summary>
    /// <param name="path">Path of the parent page</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="additionalQueryConfiguration">An action to configure additional query parameters</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A collection of web pages that exist under the specified path in the content tree</returns>
    Task<IEnumerable<T>> RetrieveWebPageChildrenByPath<T>(
        string path,
        int depth,
        bool includeSecuredItems,
        Action<RetrievePagesQueryParameters>? additionalQueryConfiguration,
        string? languageName = null)
        where T : IWebPageFieldsSource, new();

    /// <summary>
    /// Retrieves a web page content item by path without channel context using content item query API
    /// </summary>
    /// <typeparam name="T">Type of the web page to retrieve</typeparam>
    /// <param name="pathToMatch">Path where the web page lives</param>
    /// <param name="languageName">Name of the language</param>
    /// <param name="channelName">Name of the channel</param>
    /// <param name="forPreview">Indicates whether the retrieval is for preview (latest draft of items)</param>
    /// <param name="includeSecuredItems">Indicates whether to include secured items</param>
    /// <returns>The web page content item of the specified type</returns>
    Task<T?> RetrieveWebPageByPathWithoutContext<T>(
        string pathToMatch,
        string languageName,
        string channelName,
        bool forPreview,
        bool includeSecuredItems)
        where T : IWebPageFieldsSource, new();

    /// <summary>
    /// Retrieves child pages of a given web page without channel context using content item query API
    /// </summary>
    /// <param name="contentTypeNames">Content types of the child pages to retrieve</param>
    /// <param name="parentPagePath">Path of the parent page</param>
    /// <param name="customContentTypeQueryParameters">Type-level filtering through <see cref="ContentTypesQueryParameters"/></param>
    /// <param name="customContentQueryParameters">Query level filtering through <see cref="ContentQueryParameters"/></param>
    /// <param name="forPreview">Indicates whether the retrieval is for preview (latest draft of items)</param>
    /// <param name="includeSecuredItems">Indicates whether to include secured items</param>
    /// <param name="channelName">Name of the channel</param>
    /// <param name="languageName">Name of the language. If null, all languages are retrieved.</param>
    /// <param name="depth">The maximum level of recursively linked content items to include in the results. Default is 1</param>
    /// <returns>A collection of web pages that exist under the specified path in the content tree</returns>
    Task<IEnumerable<IWebPageFieldsSource>> RetrieveWebPageChildrenByPathWithoutContext(
        IEnumerable<string> contentTypeNames,
        string parentPagePath,
        Func<ContentTypesQueryParameters, ContentTypesQueryParameters> customContentTypeQueryParameters,
        Func<ContentQueryParameters, ContentQueryParameters> customContentQueryParameters,
        bool forPreview,
        bool includeSecuredItems,
        string channelName,
        string? languageName = null,
        int depth = 1);

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
    Task<IEnumerable<T>> RetrieveWebPageChildrenByPathAndReference<T>(
        string parentPagePath,
        string referenceFieldName,
        IEnumerable<int> referenceIds,
        bool includeSecuredItems = true,
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new();

    /// <summary>
    /// Retrieves reusable content item by Guid using ContentRetriever API
    /// </summary>
    /// <param name="contentItemGuid">The GUID of the reusable content item.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A reusable content item of specified type, with the specified Guid</returns>
    Task<T?> RetrieveContentItemByGuid<T>(
        Guid contentItemGuid,
        int depth = 1,
        bool includeSecuredItems = true,
        string? languageName = null)
        where T : IContentItemFieldsSource, new();

    /// <summary>
    /// Retrieves reusable content items of specified type from specified smart folder.
    /// </summary>
    /// <param name="contentTypeName">Content type name of the content items the method should return</param>
    /// <param name="smartFolderGuid">GUID of the smart folder to retrieve the content items from</param>
    /// <param name="orderBy">Order the returned items ascending/descending</param>
    /// <param name="topN">Number of items to return</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>Enumerable collection of content items from the given smart folder</returns>
    Task<IEnumerable<T>> RetrieveReusableContentItemsFromSmartFolder<T>(
        Guid smartFolderGuid,
        OrderByOption orderBy,
        int topN = 20,
        int depth = 1,
        bool includeSecuredItems = true,
        string? languageName = null)
        where T : IContentItemFieldsSource, new();

    /// <summary>
    /// Retrieves content items based on the provided schema name and tag guids.
    /// </summary>
    /// <param name="schemaName">The name of the reusable field schema</param>
    /// <param name="taxonomyColumnName">The name of the column that holds the taxonomy value</param>
    /// <param name="tagGuids">Guids of tags to filter the output by</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>An enumerable collection of content items that match the specified schema and tags</returns>
    Task<IEnumerable<IContentItemFieldsSource>> RetrieveContentItemsBySchemaAndTags(
        string schemaName,
        string taxonomyColumnName,
        IEnumerable<Guid> tagGuids,
        bool includeSecuredItems = true,
        string? languageName = null);


    Task<IEnumerable<T>> RetrieveContentItemsBySchemas<T>(
        IEnumerable<string> schemaNames,
        Action<RetrieveContentOfReusableSchemasQueryParameters> additionalQueryConfiguration,
        int depth = 1,
        bool includeSecuredItems = true,
        string? languageName = null);

    Task<IEnumerable<T>> RetrieveParentItemsOfSchema<T>(
        string schemaName,
        string referenceFieldName,
        IEnumerable<int> referenceIds,
        bool includeSecuredItems,
        int depth = 1,
        string? languageName = null);
    /// <summary>
    /// Retrieves a web page content item by path using ContentRetriever API
    /// </summary>
    /// <typeparam name="T">The type of the web page content item.</typeparam>
    /// <param name="pathToMatch">The Tree path of the web page item (can be found in the administration under the Properties tab).</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A web page content item of the specified type</returns>
    Task<T?> RetrieveWebPageByPath<T>(string pathToMatch,
        bool includeSecuredItems = true,
        string? languageName = null)
        where T : IWebPageFieldsSource, new();

    /// <summary>
    /// Retrieves a web page item by Id using the Content item query
    /// </summary>
    /// <param name="webPageItemId">The Id of the web page item</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 2.</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    Task<IWebPageFieldsSource?> RetrieveWebPageById(
        int webPageItemId,
        int depth = 2,
        bool includeSecuredItems = true,
        string? languageName = null);

    /// <summary>
    /// Retrieves a web page item by Guid using the Content item query
    /// </summary>
    /// <param name="pageContentItemGuid">The GUID of the web page item</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 2.</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    Task<IWebPageFieldsSource?> RetrieveWebPageByContentItemGuid(
        Guid pageContentItemGuid,
        int depth = 2,
        bool includeSecuredItems = true,
        string? languageName = null);
}