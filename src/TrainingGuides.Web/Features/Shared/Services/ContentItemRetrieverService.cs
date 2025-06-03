using CMS.ContentEngine;
using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc.Routing;

namespace TrainingGuides.Web.Features.Shared.Services;

public class ContentItemRetrieverService<T> : IContentItemRetrieverService<T>
{
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IWebsiteChannelContext webSiteChannelContext;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;

    public ContentItemRetrieverService(
        IContentQueryExecutor contentQueryExecutor,
        IWebsiteChannelContext webSiteChannelContext,
        IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.contentQueryExecutor = contentQueryExecutor;
        this.webSiteChannelContext = webSiteChannelContext;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }


    /// <summary>
    /// Retrieves Web page content item by Id using ContentItemQueryBuilder
    /// </summary>
    /// <param name="webPageItemId">The Id of the Web page content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns>A Web page content item of specified type, with the specifiied Id</returns>
    public async Task<T?> RetrieveWebPageById(
        int webPageItemId,
        string contentTypeName,
        int depth = 1,
        string? languageName = null)
    {
        var pages = await RetrieveWebPageContentItems(
                contentTypeName,
                innerParams => innerParams
                    .WithLinkedItems(depth),
                outerParams => outerParams
                    .Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemID), webPageItemId)),
                languageName: languageName);
        return pages.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves Web page content item by Id using ContentItemQueryBuilder
    /// </summary>
    /// <param name="webPageItemGuid">The Guid of the Web page content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns>A Web page content item of specified type, with the specifiied Id</returns>
    public async Task<T?> RetrieveWebPageByGuid(
        Guid? webPageItemGuid,
        string contentTypeName,
        int depth = 1,
        string? languageName = null)
    {
        var pages = await RetrieveWebPageContentItems(
                contentTypeName,
                innerParams => innerParams
                    .WithLinkedItems(depth),
                outerParams => outerParams
                    .Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemGUID), webPageItemGuid)),
                languageName: languageName);
        return pages.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves Web page content item by ContentItemGuid using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentItemGuid">The content item Guid of the Web page content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="language">The language of the content item. If null, the language will be inferred from the URL of the current request.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <param name="languageName">The language to query. If null, the language will be inferred from the URL of the current request.</param>
    /// <returns>A web page content item of the specified type, with the specified content item Guid</returns>
    public async Task<T?> RetrieveWebPageByContentItemGuid(
        Guid contentItemGuid,
        string contentTypeName,
        int depth = 1,
        string? languageName = null)
    {
        var pages = await RetrieveWebPageContentItems(
            contentTypeName,
            innerParams => innerParams.WithLinkedItems(depth),
            outerParams => outerParams
                .Where(where => where.WhereEquals(nameof(WebPageFields.ContentItemGUID), contentItemGuid)),
            languageName: languageName);
        return pages.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves web page content items using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="innerQueryFilter">Filter for ForContentTypes parameterization</param>
    /// <param name="outerParams">Outer query parameterization</param>
    /// <param name="languageName">Determines the language of the retrieved content. PreferredLanguageRetriever is used if empty</param>
    /// <returns>An enumerable set of items</returns>
    public async Task<IEnumerable<T>> RetrieveWebPageContentItems(
        string contentTypeName,
        Func<ContentTypesQueryParameters, ContentTypesQueryParameters> innerQueryFilter,
        Action<ContentQueryParameters> outerParams,
        string? languageName = null)
    {
        var builder = new ContentItemQueryBuilder()
                            .ForContentTypes(
                                config => innerQueryFilter(config)
                                .OfContentType(contentTypeName)
                                .WithWebPageData()
                            )
                            .Parameters(outerParams)
                            .InLanguage(languageName ?? preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentQueryExecutor.GetMappedWebPageResult<T>(builder, queryExecutorOptions);

        return pages;
    }

    /// <summary>
    /// Retrieves child pages of a given web page.
    /// </summary>
    /// <param name="parentPageContentTypeName">Content type of the parent page</param>
    /// <param name="parentPagePath">Path of the parent page</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns></returns>
    public async Task<IEnumerable<T>> RetrieveWebPageChildrenByPath(
        string parentPageContentTypeName,
        string parentPagePath,
        int depth = 1,
        string? languageName = null)
    {
        var builder = new ContentItemQueryBuilder()
                            .ForContentType(
                                parentPageContentTypeName,
                                config => config
                                    .ForWebsite(webSiteChannelContext.WebsiteChannelName, [PathMatch.Children(parentPagePath)])
                                    .WithLinkedItems(depth))
                            .InLanguage(languageName ?? preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentQueryExecutor.GetMappedWebPageResult<T>(builder, queryExecutorOptions);

        return pages;
    }

    /// <summary>
    /// Retrieves Web page content item by Id using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentItemGuid">The Guid of the reusable content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns>A Web page content item of specified type, with the specified Id</returns>
    public async Task<T?> RetrieveContentItemByGuid(
        Guid contentItemGuid,
        string contentTypeName,
        int depth = 1,
        string? languageName = null)
    {
        var items = await RetrieveReusableContentItems(
                contentTypeName ?? string.Empty,
                config => config
                    .Where(where => where.WhereEquals(nameof(ContentItemFields.ContentItemGUID), contentItemGuid))
                    .WithLinkedItems(depth),
                languageName: languageName);
        return items.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves reusable content items using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentTypeName">Content type name of the reusable item.</param>
    /// <param name="queryFilter">A delegate used to configure query for given contentTypeName</param>
    /// <returns>An enumerable set of items</returns>
    public async Task<IEnumerable<T>> RetrieveReusableContentItems(
        string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter,
        string? languageName = null)
    {
        var builder = new ContentItemQueryBuilder()
                            .ForContentType(
                                contentTypeName,
                                config => queryFilter(config)
                            )
                            .InLanguage(languageName ?? preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        var items = await contentQueryExecutor.GetMappedResult<T>(builder, queryExecutorOptions);

        return items;
    }
}

public class ContentItemRetrieverService : IContentItemRetrieverService
{
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IWebsiteChannelContext websiteChannelContext;

    public ContentItemRetrieverService(
        IContentQueryExecutor contentQueryExecutor,
        IWebsiteChannelContext websiteChannelContext)
    {
        this.contentQueryExecutor = contentQueryExecutor;
        this.websiteChannelContext = websiteChannelContext;
    }

    /// <summary>
    /// Retrieves the IWebPageFieldsSource of a web page item by Guid.
    /// </summary>
    /// <param name="webPageItemGuid">the Guid of the web page item</param>
    /// <returns><see cref="IWebPageFieldsSource"/> object containing generic <see cref="WebPageFields"/> for the item</returns>
    public async Task<IWebPageFieldsSource?> RetrieveWebPageByGuid(
        Guid webPageItemGuid)
    {
        var pages = await RetrieveWebPages(parameters =>
            {
                parameters.Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemGUID), webPageItemGuid));
            });

        return pages.FirstOrDefault();
    }

    private async Task<IEnumerable<IContentItemFieldsSource>> RetrieveContentItems(Action<ContentQueryParameters> contentQueryParameters,
        Action<ContentTypesQueryParameters> contentTypesQueryParameters)
    {
        var builder = new ContentItemQueryBuilder();

        builder.ForContentTypes(contentTypesQueryParameters)
            .Parameters(contentQueryParameters);

        return await contentQueryExecutor.GetMappedResult<IContentItemFieldsSource>(builder);
    }

    private async Task<IEnumerable<IWebPageFieldsSource>> RetrieveWebPages(Action<ContentQueryParameters> parameters)
    {
        var builder = new ContentItemQueryBuilder();

        builder.ForContentTypes(query =>
            {
                query.ForWebsite(websiteChannelContext.WebsiteChannelName);
            })
            .Parameters(parameters);

        return await contentQueryExecutor.GetMappedResult<IWebPageFieldsSource>(builder);
    }
}