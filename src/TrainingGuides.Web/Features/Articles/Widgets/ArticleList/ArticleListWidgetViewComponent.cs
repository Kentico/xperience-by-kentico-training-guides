using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using CMS.DataEngine;
using Kentico.PageBuilder.Web.Mvc;
using TrainingGuides.Web.Features.Articles.Widgets.ArticleList;
using TrainingGuides.Web.Features.Shared.Services;
using TrainingGuides.Web.Features.Articles.Services;
using CMS.ContentEngine;
using Microsoft.IdentityModel.Tokens;

[assembly:
    RegisterWidget(ArticleListWidgetViewComponent.IDENTIFIER, typeof(ArticleListWidgetViewComponent), "Article list widget",
        typeof(ArticleListWidgetProperties), Description = "Displays list of articles.", IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ArticleListWidget";

    private readonly IContentItemRetrieverService genericPageRetrieverService;
    private readonly IContentItemRetrieverService<ArticlePage> articlePageRetrieverService;

    private readonly IArticlePageService articlePageService;

    public ArticleListWidgetViewComponent(
        IContentItemRetrieverService genericPageRetrieverService,
        IContentItemRetrieverService<ArticlePage> articlePageRetrieverService,
        IArticlePageService articlePageService)
    {
        this.genericPageRetrieverService = genericPageRetrieverService;
        this.articlePageRetrieverService = articlePageRetrieverService;
        this.articlePageService = articlePageService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ArticleListWidgetProperties properties)
    {
        var model = new ArticleListWidgetViewModel();

        if (!properties.ContentTreeSection.IsNullOrEmpty())
        {
            var articlePages = await RetrieveArticlePages(properties.ContentTreeSection.First(), properties.Tags);

            model.Articles = (properties.OrderBy.Equals(OrderByOption.OldestFirst.ToString())
                ? (await GetArticlePageViewModels(articlePages)).OrderBy(article => article.CreatedOn)
                : (await GetArticlePageViewModels(articlePages)).OrderByDescending(article => article.CreatedOn))
                .Take(properties.TopN)
                .ToList();

            model.CtaText = properties.CtaText;
        }

        return View("~/Features/Articles/Widgets/ArticleList/ArticleListWidget.cshtml", model);
    }

    private async Task<IEnumerable<ArticlePage>> RetrieveArticlePages(WebPageRelatedItem parentPageSelection, IEnumerable<TagReference> tags)
    {
        var selectedPageGuid = parentPageSelection.WebPageGuid;

        var selectedPage = await genericPageRetrieverService.RetrieveWebPageByGuid(selectedPageGuid);
        string selectedPageContentTypeName = await GetWebPageContentTypeName(selectedPageGuid);
        string selectedPagePath = selectedPage?.SystemFields.WebPageItemTreePath ?? string.Empty;

        if (tags.IsNullOrEmpty())
        {
            return await articlePageRetrieverService.RetrieveWebPageChildrenByPath(
                selectedPageContentTypeName,
                selectedPagePath,
                3);
        }
        else
        {
            var tagGuids = tags.Select(tag => tag.Identifier).ToList();

            var taggedArticleIds = (
                await genericPageRetrieverService.RetrieveContentItemsBySchemaAndTags(
                    IArticleSchema.REUSABLE_FIELD_SCHEMA_NAME,
                    nameof(GeneralArticle.ArticleSchemaCategory),
                    tagGuids)
                ).Select(article => article.SystemFields.ContentItemID);

            return await articlePageRetrieverService.RetrieveWebPageChildrenByPathAndReference(
                selectedPageContentTypeName,
                selectedPagePath,
                nameof(ArticlePage.ArticlePageArticleContent),
                taggedArticleIds,
                3);
        }
    }

    private async Task<string> GetWebPageContentTypeName(Guid id)
    {
        // database-related string constants, needed to retrieve Page content type name
        const string WEB_PAGE_ITEM_OBJECT_TYPE = "cms.webpageitem";
        const string CONTENT_ITEM_TABLE_NAME = "CMS_ContentItem";
        const string WEB_PAGE_ITEM_CONTENT_ITEM_ID = "WebPageItemContentItemID";
        const string CONTENT_ITEM_ID = "ContentItemID";
        const string CONTENT_ITEM_CONTENT_TYPE_ID = "ContentItemContentTypeID";
        const string CLASS_ID = "ClassID";
        const string WEB_PAGE_ITEM_GUID = "WebPageItemGUID";
        const string CLASS_NAME = "ClassName";

        var query = new ObjectQuery(WEB_PAGE_ITEM_OBJECT_TYPE).Source(delegate (QuerySource source)
        {
            source.LeftJoin(
                source: new QuerySourceTable(CONTENT_ITEM_TABLE_NAME),
                leftColumn: WEB_PAGE_ITEM_CONTENT_ITEM_ID,
                rightColumn: CONTENT_ITEM_ID);
            source.LeftJoin<DataClassInfo>(
                leftColumn: CONTENT_ITEM_CONTENT_TYPE_ID,
                rightColumn: CLASS_ID);
        }).WhereEquals(WEB_PAGE_ITEM_GUID, id)
                .Column(CLASS_NAME);

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
