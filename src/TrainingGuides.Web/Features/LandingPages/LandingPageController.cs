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
    private readonly IContentItemRetrieverService contentItemRetrieverService;

    public LandingPageController(IContentItemRetrieverService contentItemRetrieverService)
    {
        this.contentItemRetrieverService = contentItemRetrieverService;
    }

    public async Task<IActionResult> Index()
    {
        var landingPage = await contentItemRetrieverService.RetrieveCurrentPage<LandingPage>();

        var model = LandingPageViewModel.GetViewModel(landingPage);

        return new TemplateResult(model);
    }
}