using Kentico.Content.Web.Mvc;
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
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IContentItemRetrieverService contentItemRetriever;

    public DownloadsPageController(IWebPageDataContextRetriever webPageDataContextRetriever,
        IContentItemRetrieverService contentItemRetriever)
    {
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.contentItemRetriever = contentItemRetriever;
    }

    public async Task<IActionResult> Index()
    {
        var context = webPageDataContextRetriever.Retrieve();

        var downloadsPage = await contentItemRetriever.RetrieveWebPageById<DownloadsPage>(
            context.WebPage.WebPageItemID,
            2);

        var model = DownloadsPageViewModel.GetViewModel(downloadsPage);
        return new TemplateResult(model);
    }
}