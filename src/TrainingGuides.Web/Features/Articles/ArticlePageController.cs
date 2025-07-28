using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides;
using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWebPageRoute(
    contentTypeName: ArticlePage.CONTENT_TYPE_NAME,
    controllerType: typeof(TrainingGuides.Web.Features.Articles.ArticlePageController))]

namespace TrainingGuides.Web.Features.Articles;

public class ArticlePageController : Controller
{

    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IContentItemRetrieverService<ArticlePage> articlePageRetrieverService;
    private readonly IArticlePageService articlePageService;
    private readonly IMembershipService membershipService;

    public ArticlePageController(IWebPageDataContextRetriever webPageDataContextRetriever,
        IContentItemRetrieverService<ArticlePage> articlePageRetrieverService,
        IArticlePageService articlePageService,
        IMembershipService membershipService)
    {
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.articlePageRetrieverService = articlePageRetrieverService;
        this.articlePageService = articlePageService;
        this.membershipService = membershipService;
    }

    public async Task<IActionResult> Index()
    {
        var context = webPageDataContextRetriever.Retrieve();
        var articlePage = await articlePageRetrieverService.RetrieveWebPageById(
            context.WebPage.WebPageItemID,
            ArticlePage.CONTENT_TYPE_NAME,
            2);

        if (articlePage is not null
            && articlePageService.IsReusableArticleSecured(articlePage)
            && !await membershipService.IsMemberAuthenticated())
        {
            return Forbid();
        }

        var model = await articlePageService.GetArticlePageViewModel(articlePage);
        return new TemplateResult(model);
    }
}
