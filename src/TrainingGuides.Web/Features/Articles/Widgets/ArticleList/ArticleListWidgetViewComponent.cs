using CMS.ContentEngine;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.Articles.Widgets.ArticleList;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(ArticleListWidgetViewComponent.IDENTIFIER, typeof(ArticleListWidgetViewComponent), "Article list widget",
        typeof(ArticleListWidgetProperties), Description = "Displays list of articles.", IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ArticleListWidget";

    private readonly IContentItemRetrieverService<ArticlePage> contentItemRetrieverService;
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IContentQueryResultMapper contentQueryResultMapper;
    private readonly IWebPageUrlRetriever webPageUrlRetriever;

    public ArticleListWidgetViewComponent(
        IContentItemRetrieverService<ArticlePage> contentItemRetrieverService,
        IWebPageDataContextRetriever webPageDataContextRetriever,
        IContentQueryResultMapper contentQueryResultMapper,
        IWebPageUrlRetriever webPageUrlRetriever)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.contentQueryResultMapper = contentQueryResultMapper;
        this.webPageUrlRetriever = webPageUrlRetriever;
    }

    private async Task<string> GetWebPageContentTypeName(Guid id)
    {
        // TODO: use better method provided by Kentico
        // the above comment is from KBank source code, find out if there is a better way to retrieve page class name by page guid
        var query = new ObjectQuery("cms.webpageitem").Source(delegate (QuerySource source)
        {
            source.LeftJoin<ContentItemInfo>("WebPageItemContentItemID", "ContentItemID");
            source.LeftJoin<DataClassInfo>("ContentItemContentTypeID", "ClassID");
        }).WhereEquals("WebPageItemGUID", id)
                .Column("ClassName");

        return await query.GetScalarResultAsync<string>();
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ArticleListWidgetProperties properties)
    {
        var selectedPageGuid = properties.ContentTreeSection?.Select(i => i.WebPageGuid).FirstOrDefault();

        var model = new ArticleListWidgetViewModel()
        {
            CtaText = properties.CtaText,
        };

        if (selectedPageGuid.HasValue)
        {
            string selectedPageContentTypeName = await GetWebPageContentTypeName(selectedPageGuid!.Value);
            //remove ~ form the relative path
            string selectedPagePath = (await webPageUrlRetriever.Retrieve(selectedPageGuid!.Value, "en")).RelativePath.Substring(1);

            var articlePages = await contentItemRetrieverService.RetrieveWebPageChildrenByPath(
                selectedPageContentTypeName,
                selectedPagePath,
                contentQueryResultMapper.Map<ArticlePage>,
                3
            );

            model.Articles = articlePages.Select(ArticlePageViewModel.GetViewModel)
                .Take(properties.TopN)
                .ToList();
        }

        return View("~/Features/Articles/Widgets/ArticleList/ArticleListWidget.cshtml", model);
    }
}

// Configures the query builder
// var builder = new ContentItemQueryBuilder()
//                     .ForContentType(
//                         // Scopes the query to pages of the 'My.ArticlePage' content type
//                         "My.ArticlePage",
//                         config => config
//                             // Retrieves pages only from the specified channel and path
//                             .ForWebsite(
//                                 "MyWebsiteChannel",
//                                 PathMatch.Children("/Articles"))
//                     // Retrieves only English variants of pages
//                     ).InLanguage("en");

// // Executes the query and stores the data in generated 'ArticlePage' models
// IEnumerable<ArticlePage> pages = await executor.GetWebPageResult(
//                                             builder: builder,
//                                             resultSelector: container => mapper.Map<ArticlePage>(container));

// // Displays the page data
// foreach(var page in pages)
// {
//     Console.WriteLine(page.ArticleTitle);
//     Console.WriteLine(page.ArticlePageSummary);
// }