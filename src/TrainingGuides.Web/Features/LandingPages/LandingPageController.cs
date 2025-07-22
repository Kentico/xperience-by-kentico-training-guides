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
    private readonly IContentItemRetrieverService contentItemRetriever;

    public LandingPageController(IWebPageDataContextRetriever webPageDataContextRetriever,
    IContentItemRetrieverService contentItemRetriever)
    {
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.contentItemRetriever = contentItemRetriever;
    }

    public async Task<IActionResult> Index()
    {
        var context = webPageDataContextRetriever.Retrieve();

        var landingPage = await contentItemRetriever.RetrieveWebPageById<LandingPage>
            (context.WebPage.WebPageItemID);

        var model = LandingPageViewModel.GetViewModel(landingPage);

        return new TemplateResult(model);
    }
}