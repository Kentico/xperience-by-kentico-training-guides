using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides;
using TrainingGuides.Web.Features.Shared.Services;

[assembly: RegisterWebPageRoute(
    contentTypeName: DownloadsPage.CONTENT_TYPE_NAME,
    controllerType: typeof(TrainingGuides.Web.Features.Downloads.DownloadsPageController))]

namespace TrainingGuides.Web.Features.Downloads;

public class DownloadsPageController : Controller
{
    private readonly IContentItemRetrieverService contentItemRetriever;

    public DownloadsPageController(IContentItemRetrieverService contentItemRetriever)
    {
        this.contentItemRetriever = contentItemRetriever;
    }

    public async Task<IActionResult> Index()
    {
        var downloadsPage = await contentItemRetriever.RetrieveCurrentPage<DownloadsPage>(2);

        var model = DownloadsPageViewModel.GetViewModel(downloadsPage);
        return new TemplateResult(model);
    }
}
