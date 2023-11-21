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

    public async Task<T> RetrieveWebPageById(
        int webPageItemId,
        string contentTypeName,
        Func<IWebPageContentQueryDataContainer, T> selectResult,
        int depth = 1) => await RetrieveContentItem(
            contentTypeName,
            config => config
                .Where(where => where.WhereEquals(nameof(IWebPageContentQueryDataContainer.WebPageItemID), webPageItemId))
                .WithLinkedItems(depth)
                .ForWebsite(webSiteChannelContext.WebsiteChannelName),
            selectResult);

    public async Task<T> RetrieveContentItem(
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

        IEnumerable<T> pages = await contentQueryExecutor.GetWebPageResult(builder, selectResult);

        return pages.FirstOrDefault();
    }
}
