using CMS.Websites;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides;
using TrainingGuides.Web.Services.Content;

[assembly: RegisterWebPageRoute(LandingPage.CONTENT_TYPE_NAME, typeof(TrainingGuides.Web.Components.PageTemplates.LandingPageController))]

namespace TrainingGuides.Web.Components.PageTemplates;
public class LandingPageController : Controller
{
    private readonly IWebPageDataContextRetriever webPageDataContextRetriver;
    private readonly IWebPageQueryResultMapper webPageQueryResultMapper;
    private readonly IContentItemRetrieverService<LandingPage> contentItemRetriever;

    public LandingPageController(IWebPageDataContextRetriever webPageDataContextRetriver, IWebPageQueryResultMapper webPageQueryResultMapper, IContentItemRetrieverService<LandingPage> contentItemRetriever)
    {
        this.webPageDataContextRetriver = webPageDataContextRetriver;
        this.webPageQueryResultMapper = webPageQueryResultMapper;
        this.contentItemRetriever = contentItemRetriever;
    }

    public async Task<IActionResult> Index()
    {
        var context = webPageDataContextRetriver.Retrieve();

        var landingPage = await contentItemRetriever.RetrieveWebPageById
            (context.WebPage.WebPageItemID,
            LandingPage.CONTENT_TYPE_NAME,
            container => webPageQueryResultMapper.Map<LandingPage>(container));

        var model = LandingPageViewModel.GetViewModel(landingPage);

        return new TemplateResult(model);
    }
}
