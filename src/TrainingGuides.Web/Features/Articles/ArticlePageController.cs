using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides;
using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWebPageRoute(
    contentTypeName: ArticlePage.CONTENT_TYPE_NAME,
    controllerType: typeof(TrainingGuides.Web.Features.Articles.ArticlePageController))]

namespace TrainingGuides.Web.Features.Articles;

public class ArticlePageController : Controller
{
    private readonly IContentItemRetrieverService contentItemRetrieverService;
    private readonly IArticlePageService articlePageService;

    public ArticlePageController(
        IContentItemRetrieverService contentItemRetrieverService,
        IArticlePageService articlePageService)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.articlePageService = articlePageService;
    }

    public async Task<IActionResult> Index()
    {
        var articlePage = await contentItemRetrieverService.RetrieveCurrentPage<ArticlePage>(2);

        var model = articlePageService.GetArticlePageViewModel(articlePage);
        return new TemplateResult(model);
    }
}