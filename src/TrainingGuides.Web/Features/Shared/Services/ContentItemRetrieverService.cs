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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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
                .Where(where => where.WhereEquals(nameof(ContentItemFields.ContentItemGUID), contentItemGuid)),
            languageName: languageName);
        return pages.FirstOrDefault();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;

    public ContentItemRetrieverService(
        IContentQueryExecutor contentQueryExecutor,
        IWebsiteChannelContext websiteChannelContext,
        IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.contentQueryExecutor = contentQueryExecutor;
        this.websiteChannelContext = websiteChannelContext;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }

    /// <inheritdoc />
    public async Task<IWebPageFieldsSource?> RetrieveWebPageByContentItemGuid(
        Guid pageContentItemGuid)
    {
        var pages = await RetrieveWebPages(parameters =>
            {
                parameters.Where(where => where.WhereEquals(nameof(ContentItemFields.ContentItemGUID), pageContentItemGuid));
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
        var builder = new ContentItemQueryBuilder()
            .ForContentTypes(query => query
            .WithLinkedItems(2, options => options.IncludeWebPageData(true))
            .ForWebsite(websiteChannelContext.WebsiteChannelName))
        .Parameters(parameters)
        .InLanguage(preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = websiteChannelContext.IsPreview
        };

        return await contentQueryExecutor.GetMappedResult<IWebPageFieldsSource>(builder, queryExecutorOptions);
    }
}