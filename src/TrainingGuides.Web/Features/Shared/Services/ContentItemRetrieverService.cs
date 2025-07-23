using CMS.ContentEngine;
using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using TrainingGuides.Web.Features.Shared.OptionProviders.OrderBy;

namespace TrainingGuides.Web.Features.Shared.Services;

public class ContentItemRetrieverService : IContentItemRetrieverService
{
    private readonly IContentRetriever contentRetriever;
    private readonly IWebsiteChannelContext webSiteChannelContext;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;

    public ContentItemRetrieverService(
        IContentRetriever contentRetriever,
        IWebsiteChannelContext webSiteChannelContext,
        IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.contentRetriever = contentRetriever;
        this.webSiteChannelContext = webSiteChannelContext;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }

    /// <inheritdoc />
    public async Task<T?> RetrieveWebPageById<T>(
        int webPageItemId,
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrievePagesParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentRetriever.RetrievePages<T>(
            parameters,
            query =>
                query.Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemID), webPageItemId)),
            RetrievalCacheSettings.CacheDisabled);

        return pages.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<T?> RetrieveCurrentPage<T>(
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrieveCurrentPageParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        return await contentRetriever.RetrieveCurrentPage<T>(parameters);
    }

    /// <inheritdoc />
    public async Task<T?> RetrieveWebPageByGuid<T>(
        Guid webPageItemGuid,
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrieveContentParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentRetriever.RetrieveContentByGuids<T>(
            [webPageItemGuid],
            parameters);

        return pages.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<T?> RetrieveWebPageByContentItemGuid<T>(
        Guid contentItemGuid,
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrieveContentParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentRetriever.RetrieveContentByGuids<T>(
            [contentItemGuid],
            parameters);

        return pages.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> RetrieveWebPageChildrenByPath<T>(
        string path,
        int depth = 1,
        bool includeSecuredItems = true,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrievePagesParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview,
            PathMatch = PathMatch.Children(path),
            IncludeSecuredItems = includeSecuredItems
        };

        return await contentRetriever.RetrievePages<T>(
            parameters,
            additionalQueryConfiguration: null,
            cacheSettings: RetrievalCacheSettings.CacheDisabled);
    }

    /// <inheritdoc />
    // TODO: This method requires complex query configuration that may not be directly supported by ContentRetriever API
    public async Task<IEnumerable<T>> RetrieveWebPageChildrenByPathAndReference<T>(
        string parentPageContentTypeName,
        string parentPagePath,
        string referenceFieldName,
        IEnumerable<int> referenceIds,
        bool includeSecuredItems = true,
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrievePagesParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview,
            PathMatch = PathMatch.Children(parentPagePath),
            IncludeSecuredItems = includeSecuredItems
        };

        return await contentRetriever.RetrievePages<T>(
            parameters,
            query => query.Where(where => where.WhereIn(referenceFieldName, referenceIds.ToArray())),
            RetrievalCacheSettings.CacheDisabled);
    }

    /// <inheritdoc />
    public async Task<T?> RetrieveContentItemByGuid<T>(
        Guid contentItemGuid,
        int depth = 1,
        string? languageName = null)
        where T : IContentItemFieldsSource, new()
    {
        var parameters = new RetrieveContentParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        var items = await contentRetriever.RetrieveContentByGuids<T>(
            [contentItemGuid],
            parameters);

        return items.FirstOrDefault();
    }

    /// <inheritdoc />
    // TODO: This method requires smart folder functionality that may not be directly supported by ContentRetriever API
    public async Task<IEnumerable<T>> RetrieveReusableContentItemsFromSmartFolder<T>(
        string contentTypeName,
        Guid smartFolderGuid,
        OrderByOption orderBy,
        int topN = 20,
        int depth = 1,
        string? languageName = null)
        where T : IContentItemFieldsSource, new()
    {
        // Note: Smart folder functionality may require additional query configuration
        var parameters = new RetrieveContentParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        return await contentRetriever.RetrieveContent<T>(
            parameters,
            query => query
                .OrderBy(nameof(ContentItemFields.ContentItemCommonDataLastPublishedWhen))
                .TopN(topN),
            RetrievalCacheSettings.CacheDisabled);
    }

    /// <inheritdoc />
    // TODO: This method requires complex query configuration that may not be directly supported by ContentRetriever API
    public async Task<IEnumerable<IContentItemFieldsSource>> RetrieveContentItemsBySchemaAndTags(
        string schemaName,
        string taxonomyColumnName,
        IEnumerable<Guid> tagGuids)
    {
        var parameters = new RetrieveContentParameters
        {
            LanguageName = preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        return await contentRetriever.RetrieveContent<IContentItemFieldsSource>(
            parameters,
            query => query.Where(where => where.WhereContainsTags(taxonomyColumnName, tagGuids)),
            RetrievalCacheSettings.CacheDisabled);
    }

    /// <inheritdoc />
    public async Task<IWebPageFieldsSource?> RetrieveWebPageById(int webPageItemId)
    {
        var parameters = new RetrievePagesParameters
        {
            LanguageName = preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentRetriever.RetrievePages<IWebPageFieldsSource>(
            parameters,
            query => query.Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemID), webPageItemId)),
            RetrievalCacheSettings.CacheDisabled);

        return pages.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<IWebPageFieldsSource?> RetrieveWebPageByContentItemGuid(Guid pageContentItemGuid)
    {
        var parameters = new RetrieveContentParameters
        {
            LanguageName = preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentRetriever.RetrieveContentByGuids<IWebPageFieldsSource>(
            [pageContentItemGuid],
            parameters);

        return pages.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<T?> RetrieveWebPageByPath<T>(string pathToMatch, bool includeSecuredItems = true)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrievePagesParameters
        {
            LanguageName = preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview,
            PathMatch = PathMatch.Single(pathToMatch),
            IncludeSecuredItems = includeSecuredItems
        };

        var pages = await contentRetriever.RetrievePages<T>(
            parameters,
            additionalQueryConfiguration: null,
            cacheSettings: RetrievalCacheSettings.CacheDisabled);

        return pages.FirstOrDefault();
    }
}