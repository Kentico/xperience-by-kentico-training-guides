using System.Linq;
using System.Threading.Tasks;

using CMS.ContentEngine;
using CMS.Websites.Routing;

using Kentico.Content.Web.Mvc.Routing;

using Microsoft.AspNetCore.Mvc;

namespace Kickstart.Web.Features.Navigation;

public class NavigationMenuViewComponent : ViewComponent
{
    private readonly IContentQueryExecutor contentQueryExecutor;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    private readonly IWebsiteChannelContext webSiteChannelContext;
    private readonly INavigationService navigationService;

    public NavigationMenuViewComponent(
        IContentQueryExecutor contentQueryExecutor,
        IPreferredLanguageRetriever preferredLanguageRetriever,
        IWebsiteChannelContext webSiteChannelContext,
        INavigationService navigationService)
    {
        this.contentQueryExecutor = contentQueryExecutor;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
        this.webSiteChannelContext = webSiteChannelContext;
        this.navigationService = navigationService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string navigationMenuCodeName)
    {
        var menu = await RetrieveMenu(navigationMenuCodeName);

        if (menu == null)
        {
            return View("~/Features/Navigation/NavigationMenuViewComponent.cshtml", new NavigationMenuViewModel());
        }

        var model = navigationService.GetNavigationMenuViewModel(menu);

        return View("~/Features/Navigation/NavigationMenuViewComponent.cshtml", model);
    }

    private async Task<NavigationMenu> RetrieveMenu(string navigationMenuCodeName)
    {
        var builder = new ContentItemQueryBuilder()
            .ForContentType(NavigationMenu.CONTENT_TYPE_NAME,
                config => config
                    .Where(where => where.WhereEquals(nameof(NavigationMenu.NavigationMenuCodeName), navigationMenuCodeName))
                    .WithLinkedItems(2, options => options.IncludeWebPageData(true)))
            .InLanguage(preferredLanguageRetriever.Get());


        var queryExecutorOptions = new ContentQueryExecutionOptions
        {
            ForPreview = webSiteChannelContext.IsPreview
        };

        var items = await contentQueryExecutor.GetMappedResult<NavigationMenu>(builder, queryExecutorOptions);

        return items.FirstOrDefault();
    }
}
