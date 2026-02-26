using CMS.ContentEngine;
using Kentico.Content.Web.Mvc;

namespace TrainingGuides.Web.Features.Shared.Services;

public interface IContentItemRetrieverService
{
    /// <summary>
    /// Retrieves the current page using ContentRetriever API
    /// </summary>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>The current web page content item of the specified type</returns>
    Task<T?> RetrieveCurrentPage<T>(
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new();

    /// <summary>
    /// Retrieves Web page content item by ContentItemGuid using ContentRetriever API
    /// </summary>
    /// <param name="contentItemGuid">The content item Guid of the Web page content item.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A web page content item of the specified type, with the specified content item Guid</returns>
    Task<T?> RetrieveWebPageByContentItemGuid<T>(
        Guid contentItemGuid,
        int depth = 1,
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
    /// Retrieves reusable content item by Guid using ContentRetriever API
    /// </summary>
    /// <param name="contentItemGuid">The Guid of the reusable content item.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A reusable content item of specified type, with the specified Guid</returns>
    Task<T?> RetrieveContentItemByGuid<T>(
        Guid contentItemGuid,
        int depth = 1,
        string? languageName = null)
        where T : IContentItemFieldsSource, new();

    /// <summary>
    /// Retrieves a web page item by Id using the Content item query
    /// </summary>
    /// <param name="webPageItemId">The Id of the web page item</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 2.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    Task<IWebPageFieldsSource?> RetrieveWebPageById(
        int webPageItemId,
        int depth = 2,
        string? languageName = null);

    /// <summary>
    /// Retrieves a web page item by Guid using the Content item query
    /// </summary>
    /// <param name="pageContentItemGuid">The Guid of the web page item</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 2.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    Task<IWebPageFieldsSource?> RetrieveWebPageByContentItemGuid(
        Guid pageContentItemGuid,
        int depth = 2,
        string? languageName = null);

    /// <summary>
    /// Retrieves content items based on the provided list of schema names with additional query configuration.
    /// </summary>
    /// <typeparam name="T">The type of the content items to retrieve.</typeparam>
    /// <param name="schemaNames">The list of schema names to filter the content items.</param>
    /// <param name="additionalQueryConfiguration">An action to configure additional query parameters.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>An enumerable collection of content items that match the specified schemas and query configuration.</returns>
    Task<IEnumerable<T>> RetrieveContentItemsBySchemas<T>(
        IEnumerable<string> schemaNames,
        Action<RetrieveContentOfReusableSchemasQueryParameters> additionalQueryConfiguration,
        int depth = 1,
        bool includeSecuredItems = true,
        string? languageName = null);

    /// <summary>
    /// Retrieves items that reference a given set of item IDs through the provided field, filtered by the given schema name, with additional query configuration.
    /// </summary>
    /// <typeparam name="T">The type of the content items to retrieve.</typeparam>
    /// <param name="schemaName">The name of the schema to filter the parent items.</param>
    /// <param name="referenceFieldName">The name of the field that holds the reference to the child items.</param>
    /// <param name="referenceIds">The IDs of the referenced items to filter by.</param>
    /// <param name="includeSecuredItems">If true, secured items will be included in the results.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>An enumerable collection of content items that reference the specified items.</returns>
    Task<IEnumerable<T>> RetrieveParentItemsOfSchema<T>(
        string schemaName,
        string referenceFieldName,
        IEnumerable<int> referenceIds,
        bool includeSecuredItems,
        int depth = 1,
        string? languageName = null);
}