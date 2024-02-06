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
        IPreferredLanguageRetriever preferredLanguageRetriever
        )
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
    /// <param name="resultSelector">A delegate function mapping the result of the query to the desired content type class using WebPageQueryResultMapper, e.g. webPageQueryResultMapper.Map<ArticlePage></param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns>A Web page content item of specified type, with the specifiied Id</returns>
    public async Task<T> RetrieveWebPageById(
        int webPageItemId,
        string contentTypeName,
        Func<IWebPageContentQueryDataContainer, T> resultSelector,
        int depth = 1)
    {
        var pages = await RetrieveWebPageContentItems(
                contentTypeName ?? string.Empty,
                config => config
                    .Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemID), webPageItemId))
                    .WithLinkedItems(depth),
                resultSelector);
        return pages.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves Web page content item by Id using ContentItemQueryBuilder
    /// </summary>
    /// <param name="webPageItemGuid">The Guid of the Web page content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="resultSelector">A delegate function mapping the result of the query to the desired content type class using WebPageQueryResultMapper, e.g. webPageQueryResultMapper.Map<ArticlePage></param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns>A Web page content item of specified type, with the specifiied Id</returns>
    public async Task<T> RetrieveWebPageByGuid(
        Guid webPageItemGuid,
        string contentTypeName,
        Func<IWebPageContentQueryDataContainer, T> resultSelector,
        int depth = 1)
    {
        var pages = await RetrieveWebPageContentItems(
                contentTypeName ?? string.Empty,
                config => config
                    .Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemGUID), webPageItemGuid))
                    .WithLinkedItems(depth),
                resultSelector);
        return pages.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves web page content items using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="queryFilter">A delegate used to configure query for given contentTypeName</param>
    /// <param name="resultSelector">A delegate function mapping the result of the query to the desired content type class using WebPageQueryResultMapper, e.g. webPageQueryResultMapper.Map<ArticlePage>.</param>
    /// <returns></returns>
    public async Task<IEnumerable<T>> RetrieveWebPageContentItems(
        string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter,
        Func<IWebPageContentQueryDataContainer, T> resultSelector)
    {
        var builder = new ContentItemQueryBuilder()
                            .ForContentType(
                                contentTypeName,
                                config => queryFilter(config)
                                .ForWebsite(webSiteChannelContext.WebsiteChannelName)
                            )
                            .InLanguage(preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentQueryExecutor.GetWebPageResult(builder, resultSelector, queryExecutorOptions);

        return pages;
    }



    /// <summary>
    /// Retrieves Web page content item by Id using ContentItemQueryBuilder
    /// </summary>
    /// <param name="contentItemGuid">The Guid of the reusable content item.</param>
    /// <param name="contentTypeName">Content type name of the Web page.</param>
    /// <param name="resultSelector">A delegate function mapping the result of the query to the desired content type class using WebPageQueryResultMapper, e.g. webPageQueryResultMapper.Map<ArticlePage></param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns>A Web page content item of specified type, with the specifiied Id</returns>
    public async Task<T> RetrieveContentItemByGuid(
        Guid contentItemGuid,
        string contentTypeName,
        Func<IContentQueryDataContainer, T> resultSelector,
        int depth = 1)
    {
        var items = await RetrieveReusableContentItems(
                contentTypeName ?? string.Empty,
                config => config
                    .Where(where => where.WhereEquals(nameof(ContentItemFields.ContentItemGUID), contentItemGuid))
                    .WithLinkedItems(depth),
                resultSelector);
        return items.FirstOrDefault();
    }

    public async Task<IEnumerable<T>> RetrieveReusableContentItems(
        string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> queryFilter,
        Func<IContentQueryDataContainer, T> resultSelector)
    {
        var builder = new ContentItemQueryBuilder()
                            .ForContentType(
                                contentTypeName,
                                config => queryFilter(config)
                            )
                            .InLanguage(preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        var items = await contentQueryExecutor.GetResult(builder, resultSelector, queryExecutorOptions);

        return items;
    }
}


public class ContentItemRetrieverService : IContentItemRetrieverService
{
    private readonly Dictionary<string, Func<IWebPageContentQueryDataContainer, IWebPageFieldsSource>> contentTypeDictionary;

    private readonly IWebPageQueryResultMapper webPageQueryResultMapper;
    private readonly IContentItemRetrieverService<IWebPageFieldsSource> contentItemRetrieverService;

    public ContentItemRetrieverService(IWebPageQueryResultMapper webPageQueryResultMapper, IContentItemRetrieverService<IWebPageFieldsSource> contentItemRetrieverService)
    {
        this.webPageQueryResultMapper = webPageQueryResultMapper;
        this.contentItemRetrieverService = contentItemRetrieverService;

        contentTypeDictionary = new Dictionary<string, Func<IWebPageContentQueryDataContainer, IWebPageFieldsSource>>
        {
            { ArticlePage.CONTENT_TYPE_NAME, container => this.webPageQueryResultMapper.Map<ArticlePage>(container) },
            { DownloadsPage.CONTENT_TYPE_NAME, container => this.webPageQueryResultMapper.Map<DownloadsPage>(container) },
            { EmptyPage.CONTENT_TYPE_NAME, container => this.webPageQueryResultMapper.Map<EmptyPage>(container) },
            { LandingPage.CONTENT_TYPE_NAME, container => this.webPageQueryResultMapper.Map<LandingPage>(container) },
            { ProductPage.CONTENT_TYPE_NAME, container => this.webPageQueryResultMapper.Map<ProductPage>(container) },
        };
    }

    public async Task<IWebPageFieldsSource> RetrieveWebPageById(
        int webPageItemId,
        string contentTypeName) => await
            contentItemRetrieverService.RetrieveWebPageById(
                webPageItemId,
                contentTypeName,
                contentTypeDictionary[contentTypeName]);

    public async Task<IWebPageFieldsSource> RetrieveWebPageByGuid(
        Guid webPageItemGuid,
        string contentTypeName) => await
            contentItemRetrieverService.RetrieveWebPageByGuid(
                webPageItemGuid,
                contentTypeName,
                contentTypeDictionary[contentTypeName]);
}
