using System.Threading.Tasks;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using Kickstart;

using Microsoft.AspNetCore.Mvc;

[assembly: RegisterWebPageRoute(
    contentTypeName: LandingPage.CONTENT_TYPE_NAME,
    controllerType: typeof(Kickstart.Web.Features.LandingPages.LandingPageController))]

namespace Kickstart.Web.Features.LandingPages;

public class LandingPageController : Controller
{
    private readonly IContentRetriever contentRetriever;

    public LandingPageController(
        IContentRetriever contentRetriever) => this.contentRetriever = contentRetriever;

    public async Task<IActionResult> Index()
    {
        var parameters = new RetrieveCurrentPageParameters
        {
            LinkedItemsMaxLevel = 1
        };
        var page = await contentRetriever.RetrieveCurrentPage<LandingPage>(parameters);
        var model = LandingPageViewModel.GetViewModel(page);
        return new TemplateResult(model);
    }
}
