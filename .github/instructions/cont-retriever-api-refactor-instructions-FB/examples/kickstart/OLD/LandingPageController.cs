using System.Linq;
using System.Threading.Tasks;

using CMS.ContentEngine;
using CMS.Websites;
using CMS.Websites.Routing;

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
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IWebPageDataContextRetriever webPageDataContextRetriever;
    private readonly IWebsiteChannelContext webSiteChannelContext;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;

    public LandingPageController(
        IContentQueryExecutor contentQueryExecutor,
        IWebPageDataContextRetriever webPageDataContextRetriever,
        IWebsiteChannelContext webSiteChannelContext,
        IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.contentQueryExecutor = contentQueryExecutor;
        this.webPageDataContextRetriever = webPageDataContextRetriever;
        this.webSiteChannelContext = webSiteChannelContext;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }

    public async Task<IActionResult> Index()
    {
        var context = webPageDataContextRetriever.Retrieve();
        var builder = new ContentItemQueryBuilder()
                            .ForContentType(
                                LandingPage.CONTENT_TYPE_NAME,
                                config => config
                                    .Where(where => where.WhereEquals(nameof(WebPageFields.WebPageItemID), context.WebPage.WebPageItemID))
                                    .WithLinkedItems(1)
                                    .ForWebsite(webSiteChannelContext.WebsiteChannelName)
                                )
                            .InLanguage(preferredLanguageRetriever.Get());

        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        var pages = await contentQueryExecutor.GetMappedWebPageResult<LandingPage>(builder, queryExecutorOptions);

        var model = LandingPageViewModel.GetViewModel(pages.FirstOrDefault());
        return new TemplateResult(model);
    }
}
