using Kentico.Content.Web.Mvc;
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
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IContentItemRetrieverService articlePageRetrieverService;
    private readonly IArticlePageService articlePageService;

    public ArticlePageController(IWebPageDataContextRetriever webPageDataContextRetriever,
        IContentItemRetrieverService articlePageRetrieverService,
        IArticlePageService articlePageService)
    {
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.articlePageRetrieverService = articlePageRetrieverService;
        this.articlePageService = articlePageService;
    }

    public async Task<IActionResult> Index()
    {
        var context = webPageDataContextRetriever.Retrieve();
        var articlePage = await articlePageRetrieverService.RetrieveWebPageById<ArticlePage>(
            context.WebPage.WebPageItemID,
            2);

        var model = await articlePageService.GetArticlePageViewModel(articlePage);
        return new TemplateResult(model);
    }
}