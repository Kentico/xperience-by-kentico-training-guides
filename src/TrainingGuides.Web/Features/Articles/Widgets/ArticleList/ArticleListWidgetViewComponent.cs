using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.IdentityModel.Tokens;
using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Articles.Widgets.ArticleList;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(ArticleListWidgetViewComponent.IDENTIFIER, typeof(ArticleListWidgetViewComponent), "Article list widget",
        typeof(ArticleListWidgetProperties), Description = "Displays list of articles.", IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ArticleListWidget";

    private readonly IContentItemRetrieverService contentItemRetrieverService;
    private readonly IArticlePageService articlePageService;

    public ArticleListWidgetViewComponent(
        IContentItemRetrieverService contentItemRetrieverService,
        IArticlePageService articlePageService)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.articlePageService = articlePageService;
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ArticleListWidgetProperties properties)
    {
        var model = new ArticleListWidgetViewModel();

        if (!properties.ContentTreeSection.IsNullOrEmpty())
        {
            var articlePages = await RetrieveArticlePages(properties.ContentTreeSection.First());

            model.Articles = (properties.OrderBy.Equals("OldestFirst", StringComparison.OrdinalIgnoreCase)
                ? (await GetArticlePageViewModels(articlePages)).OrderBy(article => article.CreatedOn)
                : (await GetArticlePageViewModels(articlePages)).OrderByDescending(article => article.CreatedOn))
                .Take(properties.TopN)
                .ToList();

            model.CtaText = properties.CtaText;
        }

        return View("~/Features/Articles/Widgets/ArticleList/ArticleListWidget.cshtml", model);
    }

    private async Task<IEnumerable<ArticlePage>> RetrieveArticlePages(ContentItemReference parentPageSelection)
    {
        var selectedPageGuid = parentPageSelection.Identifier;

        var selectedPage = selectedPageGuid != Guid.Empty
            ? await contentItemRetrieverService.RetrieveWebPageByContentItemGuid<ArticlePage>(selectedPageGuid)
            : null;

        string selectedPagePath = selectedPage?.SystemFields.WebPageItemTreePath ?? string.Empty;

        if (string.IsNullOrEmpty(selectedPagePath))
        {
            return Enumerable.Empty<ArticlePage>();
        }

        return await contentItemRetrieverService.RetrieveWebPageChildrenByPath<ArticlePage>(
            selectedPagePath,
            3);
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
