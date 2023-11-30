using CMS.Websites;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWebPageRoute(
    contentTypeName: ArticlePage.CONTENT_TYPE_NAME,
    controllerType: typeof(TrainingGuides.Web.Features.Articles.ArticlePageController))]

namespace TrainingGuides.Web.Features.Articles;
public class ArticlePageController : Controller
{

    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IWebPageQueryResultMapper webPageQueryResultMapper;
    private readonly IContentItemRetrieverService<ArticlePage> contentItemRetriever;

    public ArticlePageController(IWebPageDataContextRetriever webPageDataContextRetriever,
        IWebPageQueryResultMapper webPageQueryResultMapper,
        IContentItemRetrieverService<ArticlePage> contentItemRetriever)
    {
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.webPageQueryResultMapper = webPageQueryResultMapper;
        this.contentItemRetriever = contentItemRetriever;
    }

    public async Task<IActionResult> Index()
    {
        var context = webPageDataContextRetriever.Retrieve();
        var articlePage = await contentItemRetriever.RetrieveWebPageById(
            context.WebPage.WebPageItemID,
            ArticlePage.CONTENT_TYPE_NAME,
            /*
             * Not something you need to change, but whenever you have a
             * delegate that passes a single parameter to a method that only takes
             * that parameter (ex: x => method(x)) you can just pass the
             * delegate as a parameter instead.
             * In the case below you would pass webPageQueryResultMapper.Map<ArticlePage>
             * as the parameter
             */
            container => webPageQueryResultMapper.Map<ArticlePage>(container));

        var model = ArticlePageViewModel.GetViewModel(articlePage);
        return new TemplateResult(model);
    }
}
