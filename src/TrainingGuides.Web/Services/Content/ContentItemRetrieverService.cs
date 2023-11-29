using CMS.ContentEngine;
using CMS.Websites;
using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc.Routing;

namespace TrainingGuides.Web.Services.Content;

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
    /// <param name="selectResult">A function mapping the result of the query to the desired content type class using WebPageQueryResultMapper, e.g. container => webPageQueryResultMapper.Map<ArticlePage>(container)</param>
    /// <param name="depth">The maximum level of recursively linked content items that should be included in the results. Default value is 1.</param>
    /// <returns>A Web page content item of specified type</returns>
    public async Task<T> RetrieveWebPageById(int webPageItemId, string contentTypeName, Func<IWebPageContentQueryDataContainer, T> selectResult, int depth = 1) => await RetrieveWebPageContentItem(
            contentTypeName,
            config => config
                .Where(where => where.WhereEquals(nameof(IWebPageContentQueryDataContainer.WebPageItemID), webPageItemId))
                .WithLinkedItems(depth)
                .ForWebsite(webSiteChannelContext.WebsiteChannelName),
            selectResult);

    private async Task<T> RetrieveWebPageContentItem(
        string contentTypeName,
        Func<ContentTypeQueryParameters, ContentTypeQueryParameters> filterQuery,
        Func<IWebPageContentQueryDataContainer, T> selectResult)
    {
        var builder = new ContentItemQueryBuilder()
                            .ForContentType(
                                contentTypeName,
                                config => filterQuery(config)
                            )
                            .InLanguage(preferredLanguageRetriever.Get());

        var pages = await contentQueryExecutor.GetWebPageResult(builder, selectResult);

        return pages.FirstOrDefault();
    }
}
