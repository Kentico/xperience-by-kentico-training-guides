using CMS.ContentEngine;
using CMS.DataEngine;
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
    private readonly IContentQueryExecutor contentQueryExecutor;

    public ContentItemRetrieverService(
        IContentRetriever contentRetriever,
        IWebsiteChannelContext webSiteChannelContext,
        IPreferredLanguageRetriever preferredLanguageRetriever,
        IContentQueryExecutor contentQueryExecutor)
    {
        this.contentRetriever = contentRetriever;
        this.webSiteChannelContext = webSiteChannelContext;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
        this.contentQueryExecutor = contentQueryExecutor;
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
        Action<RetrievePagesQueryParameters>? additionalQueryConfiguration = null,
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
            additionalQueryConfiguration: additionalQueryConfiguration,
            cacheSettings: RetrievalCacheSettings.CacheDisabled);
    }

    public async Task<IEnumerable<T>> RetrieveWebPageChildrenByPathAndReference<T>(
        string parentPageContentTypeName,
        string parentPagePath,
        string referenceFieldName,
        IEnumerable<int> referenceIds,
        bool includeSecuredItems,
        int depth = 1,
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
     => await RetrieveWebPageChildrenByPath<T>(
            path: parentPagePath,
            includeSecuredItems: includeSecuredItems,
            depth: depth,
            additionalQueryConfiguration: config => config.Linking(referenceFieldName, referenceIds),
            languageName: languageName);

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
    public async Task<IEnumerable<T>> RetrieveReusableContentItemsFromSmartFolder<T>(
        string contentTypeName,
        Guid smartFolderGuid,
        OrderByOption orderBy,
        int topN = 20,
        int depth = 1,
        string? languageName = null)
        where T : IContentItemFieldsSource, new()
    {
        const string LAST_PUBLISHED_COLUMN_NAME = "ContentItemCommonDataLastPublishedWhen";

        Action<ContentTypesQueryParameters> contentTypesQueryParameters = parameters => parameters
            .InSmartFolder(smartFolderGuid)
            .OfContentType(contentTypeName)
            .WithLinkedItems(depth)
            .WithContentTypeFields();

        Action<ContentQueryParameters> contentQueryParameters = parameters
            => parameters
                .OrderBy(new OrderByColumn(
                    LAST_PUBLISHED_COLUMN_NAME,
                    orderBy.Equals(OrderByOption.NewestFirst) ? OrderDirection.Descending : OrderDirection.Ascending))
                .TopN(topN);

        var parameters = new RetrieveContentParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        var a = await contentRetriever.RetrieveContent<T>(
            parameters,
            query => query
                .InSmartFolder(smartFolderGuid)
                .OrderBy(new OrderByColumn(
                    LAST_PUBLISHED_COLUMN_NAME,
                    orderBy.Equals(OrderByOption.NewestFirst) ? OrderDirection.Descending : OrderDirection.Ascending))
                .TopN(topN),
            RetrievalCacheSettings.CacheDisabled);

        return a;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IContentItemFieldsSource>> RetrieveContentItemsBySchemaAndTags(
        string schemaName,
        string taxonomyColumnName,
        IEnumerable<Guid> tagGuids)
    {
        var parameters = new RetrieveContentOfReusableSchemasParameters
        {
            LanguageName = preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview
        };

        return await contentRetriever.RetrieveContentOfReusableSchemas<IContentItemFieldsSource>(
            [schemaName],
            parameters,
            query => query.Where(where => where.WhereContainsTags(taxonomyColumnName, tagGuids)),
            RetrievalCacheSettings.CacheDisabled,
            configureModel: null);
    }

    /// <inheritdoc />
    /// using the "old way" of retrieving content here, because we don't know the exact page type of the webpage item requested. Most of the new ContentRetriever API methods require a specific type and won't accept an interface,
    /// e.g., contentRetriever.RetrieveContentByGuids<IWebPageFieldsSource>.
    ///
    /// An alternative method could be implemented using the ContentRetriever API, but it would still require a listing of all page types:
    /// var pages = await contentRetriever.RetrievePagesOfContentTypes<IWebPageFieldsSource>(
    //      [ List of all pages content types ],
    //      parameters,
    //      query => query.Where(where => where.WhereEquals(nameof(ContentItemFields.ContentItemGUID), pageContentItemGuid)),
    //      RetrievalCacheSettings.CacheDisabled,
    //      configureModel: null);
    public async Task<IWebPageFieldsSource?> RetrieveWebPageByContentItemGuid(
        Guid pageContentItemGuid)
    {
        var pages = await RetrieveWebPages(parameters =>
            {
                parameters.Where(where => where.WhereEquals(nameof(ContentItemFields.ContentItemGUID), pageContentItemGuid));
            });

        return pages.FirstOrDefault();
    }

    private async Task<IEnumerable<IWebPageFieldsSource>> RetrieveWebPages(Action<ContentQueryParameters> parameters)
    {
        var builder = new ContentItemQueryBuilder()
            .ForContentTypes(query => query
            .WithLinkedItems(2, options => options.IncludeWebPageData(true))
            .ForWebsite(webSiteChannelContext.WebsiteChannelName))
        .Parameters(parameters)
        .InLanguage(preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        return await contentQueryExecutor.GetMappedResult<IWebPageFieldsSource>(builder, queryExecutorOptions);
    }

    /// <inheritdoc />
    /// see comment at Task<IWebPageFieldsSource?> RetrieveWebPageByContentItemGuid
    public async Task<IWebPageFieldsSource?> RetrieveWebPageById(
        int webPageItemId)
    {
        var pages = await RetrieveWebPages(parameters =>
            {
                parameters.Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemID), webPageItemId));
            });

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