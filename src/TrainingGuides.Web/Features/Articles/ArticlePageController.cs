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

    private readonly IContentItemRetrieverService contentItemRetrieverService;
    private readonly IArticlePageService articlePageService;
    private readonly IMembershipService membershipService;

    public ArticlePageController(
        IContentItemRetrieverService contentItemRetrieverService,
        IArticlePageService articlePageService,
        IMembershipService membershipService)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
        this.articlePageService = articlePageService;
        this.membershipService = membershipService;
    }

    public async Task<IActionResult> Index()
    {
        var articlePage = await contentItemRetrieverService.RetrieveCurrentPage<ArticlePage>(2);

        if (articlePage is not null
            && articlePageService.IsReusableArticleSecured(articlePage)
            && !await membershipService.IsMemberAuthenticated())
        {
            return Forbid();
        }

        var model = articlePageService.GetArticlePageViewModel(articlePage);
        return new TemplateResult(model);
    }
}
