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
    /// <param name="selectResult">A delegate function mapping the result of the query to the desired content type class using WebPageQueryResultMapper, e.g. webPageQueryResultMapper.Map<ArticlePage></param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns>A Web page content item of specified type</returns>
    public async Task<T> RetrieveWebPageById(
        int webPageItemId,
        string contentTypeName,
        Func<IWebPageContentQueryDataContainer, T> selectResult,
        int depth = 1) => await
            RetrieveWebPageContentItem(
                contentTypeName: contentTypeName,
                filterQuery: config => config
                    .Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemID), webPageItemId))
                    .WithLinkedItems(depth)
                    .ForWebsite(webSiteChannelContext.WebsiteChannelName),
                selectResult: selectResult);

    private async Task<T> RetrieveWebPageContentItem(
        string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> filterQuery,
        Func<IWebPageContentQueryDataContainer, T> selectResult)
    {
        var builder = new ContentItemQueryBuilder()
                            .ForContentType(
                                contentTypeName: contentTypeName,
                                configureQuery: config => filterQuery(config)
                            )
                            .InLanguage(preferredLanguageRetriever.Get());

        var pages = await contentQueryExecutor.GetWebPageResult(
            builder: builder,
            resultSelector: selectResult);

        return pages.FirstOrDefault();
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
            { LandingPage.CONTENT_TYPE_NAME, container => this.webPageQueryResultMapper.Map<LandingPage>(container) }
        };
    }

    public async Task<IWebPageFieldsSource> RetrieveWebPageById(
        int webPageItemId,
        string contentTypeName) => await
            contentItemRetrieverService.RetrieveWebPageById(
                webPageItemId: webPageItemId,
                contentTypeName: contentTypeName,
                resultSelector: contentTypeDictionary[contentTypeName]);
}
