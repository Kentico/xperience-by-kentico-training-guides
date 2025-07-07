using CMS.ContentEngine;

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
    Task<T?> RetrieveWebPageById(
        int webPageItemId,
        string contentTypeName,
        int depth = 1,
        string? languageName = null);

    /// <summary>
    /// Retrieves Web page content item by Id using ContentItemQueryBuilder
    /// </summary>
    /// <param name="webPageItemGuid">The Guid of the Web page content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns>A Web page content item of specified type, with the specified Id</returns>
    Task<T?> RetrieveWebPageByGuid(
        Guid? webPageItemGuid,
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
    /// Retrieves web page content items using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="innerQueryFilter">Filter for ForContentTypes parameterization</param>
    /// <param name="outerParams">Outer query parameterization</param>
    /// <param name="languageName">Determines the language of the retrieved content. PreferredLanguageRetriever is used if empty</param>
    /// <returns>An enumerable set of items</returns>
    Task<IEnumerable<T>> RetrieveWebPageContentItems(
        string contentTypeName,
        Func<ContentTypesQueryParameters, ContentTypesQueryParameters> innerQueryFilter,
        Action<ContentQueryParameters> outerParams,
        string? languageName = null);

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
        int depth = 1,
        string? languageName = null);

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
        int depth = 1,
        string? languageName = null);

    /// <summary>
    /// Retrieves reusable content items using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentTypeName">Content type name of the reusable item.</param>
    /// <param name="queryFilter">A delegate used to configure query for given contentTypeName</param>
    /// <returns>An enumerable set of items</returns>
    Task<IEnumerable<T>> RetrieveReusableContentItems(
        string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter,
        string? languageName = null);
}

public interface IContentItemRetrieverService
{
    /// <summary>
    /// Retrieves the IWebPageFieldsSource of a web page item by Guid.
    /// </summary>
    /// <param name="pageContentItemGuid">the Guid of the web page item</param>
    /// <returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    Task<IWebPageFieldsSource?> RetrieveWebPageByContentItemGuid(Guid pageContentItemGuid);

}
