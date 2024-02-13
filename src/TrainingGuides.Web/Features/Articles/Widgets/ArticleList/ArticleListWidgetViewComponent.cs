using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using CMS.ContentEngine.Internal;
using CMS.DataEngine;
using Kentico.PageBuilder.Web.Mvc;
using TrainingGuides.Web.Features.Articles.Entities;
using TrainingGuides.Web.Features.Articles.Widgets.ArticleList;
using TrainingGuides.Web.Features.Shared.Services;
using TrainingGuides.Web.Features.Articles.Services;

[assembly:
    RegisterWidget(ArticleListWidgetViewComponent.IDENTIFIER, typeof(ArticleListWidgetViewComponent), "Article list widget",
        typeof(ArticleListWidgetProperties), Description = "Displays list of articles.", IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ArticleListWidget";

    private readonly IContentItemRetrieverService<GenericPage> genericPageRetrieverService;
    private readonly IContentItemRetrieverService<ArticlePage> articlePageRetrieverService;
    private readonly IWebPageQueryResultMapper webPageQueryResultMapper;

    private readonly IArticlePageService articlePageService;

    public ArticleListWidgetViewComponent(
        IContentItemRetrieverService<GenericPage> genericPageRetrieverService,
        IContentItemRetrieverService<ArticlePage> articlePageRetrieverService,
        IWebPageQueryResultMapper webPageQueryResultMapper,
        IArticlePageService articlePageService)
    {
        this.genericPageRetrieverService = genericPageRetrieverService;
        this.articlePageRetrieverService = articlePageRetrieverService;
        this.webPageQueryResultMapper = webPageQueryResultMapper;
        this.articlePageService = articlePageService;
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

            var parentPage = await genericPageRetrieverService.RetrieveWebPageByGuid(
                selectedPageGuid,
                selectedPageContentTypeName,
                webPageQueryResultMapper.Map<GenericPage>);

            string selectedPagePath = parentPage.SystemFields.WebPageItemTreePath;

            var articlePages = await articlePageRetrieverService.RetrieveWebPageChildrenByPath(
                selectedPageContentTypeName,
                selectedPagePath,
                webPageQueryResultMapper.Map<ArticlePage>,
                3);

            model.Articles = (properties.OrderBy.Equals("oldest-first")
                ? (await GetArticlePageViewModels(articlePages)).OrderBy(article => article.CreatedOn)
                : (await GetArticlePageViewModels(articlePages)).OrderByDescending(article => article.CreatedOn))
                .Take(properties.TopN)
                .ToList();
        }

        return View("~/Features/Articles/Widgets/ArticleList/ArticleListWidget.cshtml", model);
    }

    private async Task<string> GetWebPageContentTypeName(Guid id)
    {
        var query = new ObjectQuery("cms.webpageitem").Source(delegate (QuerySource source)
        {
            source.LeftJoin<ContentItemInfo>("WebPageItemContentItemID", "ContentItemID");
            source.LeftJoin<DataClassInfo>("ContentItemContentTypeID", "ClassID");
        }).WhereEquals("WebPageItemGUID", id)
                .Column("ClassName");

        return await query.GetScalarResultAsync<string>();
    }

    private async Task<List<ArticlePageViewModel>> GetArticlePageViewModels(IEnumerable<ArticlePage?>? articlePages)
    {
        var models = new List<ArticlePageViewModel>();
        if (articlePages != null)
        {
            foreach (var articlePage in articlePages)
            {
                if (articlePage != null)
                {
                    var model = await articlePageService.GetArticlePageViewModel(articlePage);
                    models.Add(model);
                }
            }
        }
        return models;
    }
}
