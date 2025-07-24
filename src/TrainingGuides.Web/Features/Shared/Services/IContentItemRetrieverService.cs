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
    /// Retrieves web page content items using ContentRetriever API
    /// </summary>
    /// <param name="additionalQueryConfiguration">Additional query configuration for filtering and ordering</param>
    /// <param name="languageName">Determines the language of the retrieved content. PreferredLanguageRetriever is used if empty</param>
    /// <returns>An enumerable set of items</returns>
    Task<IEnumerable<T>> RetrieveWebPageContentItems<T>(
        Action<RetrievePagesQueryParameters>? additionalQueryConfiguration = null,
        string? languageName = null)
        where T : IWebPageFieldsSource, new();

    /// <summary>
    /// Retrieves child pages of a given web page.
    /// </summary>
    /// <param name="path">Path of the parent page</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>An enumerable collection of child web page content items of the specified type</returns>
    Task<IEnumerable<T>> RetrieveWebPageChildrenByPath<T>(
        string path,
        int depth = 1,
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
    /// Retrieves reusable content items using ContentRetriever API
    /// </summary>
    /// <param name="additionalQueryConfiguration">Additional query configuration for filtering and ordering</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>An enumerable collection of reusable content items of the specified type</returns>
    Task<IEnumerable<T>> RetrieveReusableContentItems<T>(
        Action<RetrieveContentQueryParameters>? additionalQueryConfiguration = null,
        string? languageName = null)
        where T : IContentItemFieldsSource, new();

    /// <summary>
    /// Retrieves the IWebPageFieldsSource of a web page item by Id.
    /// </summary>
    /// <param name="webPageItemId">The Id of the web page item</param>
    /// <returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    Task<IWebPageFieldsSource?> RetrieveWebPageById(int webPageItemId);

    /// <summary>
    /// Retrieves the IWebPageFieldsSource of a web page item by Guid.
    /// </summary>
    /// <param name="pageContentItemGuid">the Guid of the web page item</param>
    /// <returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    Task<IWebPageFieldsSource?> RetrieveWebPageByContentItemGuid(Guid pageContentItemGuid);
}
