using CMS.ContentEngine;
using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;

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
        string? languageName = null)
        where T : IWebPageFieldsSource, new()
    {
        var parameters = new RetrievePagesParameters
        {
            LinkedItemsMaxLevel = depth,
            LanguageName = languageName ?? preferredLanguageRetriever.Get(),
            IsForPreview = webSiteChannelContext.IsPreview,
            PathMatch = PathMatch.Children(path)
        };

        return await contentRetriever.RetrievePages<T>(parameters);
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
        Guid pageContentItemGuid,
        string? languageName = null)
    {
        var pages = await RetrieveWebPages(parameters =>
            {
                parameters.Where(where => where.WhereEquals(nameof(ContentItemFields.ContentItemGUID), pageContentItemGuid));
            }, languageName);

        return pages.FirstOrDefault();
    }

    private async Task<IEnumerable<IWebPageFieldsSource>> RetrieveWebPages(
        Action<ContentQueryParameters> parameters,
        string? languageName = null)
    {
        var builder = new ContentItemQueryBuilder()
            .ForContentTypes(query => query
            .WithLinkedItems(2, options => options.IncludeWebPageData(true))
            .ForWebsite(webSiteChannelContext.WebsiteChannelName))
        .Parameters(parameters)
        .InLanguage(languageName ?? preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        return await contentQueryExecutor.GetMappedResult<IWebPageFieldsSource>(builder, queryExecutorOptions);
    }

    /// <inheritdoc />
    /// see the comment at the RetrieveWebPageByContentItemGuid method
    public async Task<IWebPageFieldsSource?> RetrieveWebPageById(
        int webPageItemId,
        string? languageName = null)
    {
        var pages = await RetrieveWebPages(parameters =>
            {
                parameters.Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemID), webPageItemId));
            }, languageName);

        return pages.FirstOrDefault();
    }

}