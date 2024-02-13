using Kentico.Xperience.Admin.Websites;

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedNews;

public class NewsWidgetsItemModifier : IWebPagePanelItemModifier
{
    public WebPagePanelItem Modify(WebPagePanelItem webPagePanelItem,
        WebPagePanelItemModifierParameters webPagePanelItemModifierParameters)
    {
        webPagePanelItem.SelectableOption.Selectable = true;
        return webPagePanelItem;
    }
}
