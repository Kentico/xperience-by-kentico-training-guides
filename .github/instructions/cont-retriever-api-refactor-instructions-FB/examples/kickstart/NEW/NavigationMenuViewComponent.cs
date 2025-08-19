using System.Linq;
using System.Threading.Tasks;

using Kentico.Content.Web.Mvc;

using Microsoft.AspNetCore.Mvc;

namespace Kickstart.Web.Features.Navigation;

public class NavigationMenuViewComponent : ViewComponent
{
    private readonly IContentRetriever contentRetriever;

    public NavigationMenuViewComponent(IContentRetriever contentRetriever) => this.contentRetriever = contentRetriever;

    public async Task<IViewComponentResult> InvokeAsync(string navigationMenuCodeName)
    {
        var menu = await RetrieveMenu(navigationMenuCodeName);

        if (menu == null)
        {
            return View("~/Features/Navigation/NavigationMenuViewComponent.cshtml", new NavigationMenuViewModel());
        }

        var model = NavigationMenuViewModel.GetViewModel(menu);

        return View("~/Features/Navigation/NavigationMenuViewComponent.cshtml", model);
    }

    private async Task<NavigationMenu> RetrieveMenu(string navigationMenuCodeName)
    {
        var parameters = new RetrieveContentParameters
        {
            LinkedItemsMaxLevel = 2
        };
        var menus = await contentRetriever.RetrieveContent<NavigationMenu>(
            parameters,
            query => query
                .Where(where => where.WhereEquals(nameof(NavigationMenu.NavigationMenuCodeName), navigationMenuCodeName)),
            RetrievalCacheSettings.CacheDisabled
            );

        return menus.FirstOrDefault();
    }
}
