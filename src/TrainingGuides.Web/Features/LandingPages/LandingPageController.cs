using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWebPageRoute(
    contentTypeName: LandingPage.CONTENT_TYPE_NAME,
    controllerType: typeof(TrainingGuides.Web.Features.LandingPages.LandingPageController))]

namespace TrainingGuides.Web.Features.LandingPages;
public class LandingPageController : Controller
{
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IContentItemRetrieverService<LandingPage> contentItemRetriever;

    public LandingPageController(
        IWebPageDataContextRetriever webPageDataContextRetriever,
        IContentItemRetrieverService<LandingPage> contentItemRetriever)
    {
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.contentItemRetriever = contentItemRetriever;
    }

    public async Task<IActionResult> Index()
    {
        var context = webPageDataContextRetriever.Retrieve();

        var landingPage = await contentItemRetriever.RetrieveWebPageById
            (context.WebPage.WebPageItemID,
            LandingPage.CONTENT_TYPE_NAME);

        var model = LandingPageViewModel.GetViewModel(landingPage);

        return new TemplateResult(model);
    }
}
