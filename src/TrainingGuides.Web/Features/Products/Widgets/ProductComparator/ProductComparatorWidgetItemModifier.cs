using Kentico.Xperience.Admin.Websites;

namespace TrainingGuides.Web.Features.Products.Widgets.ProductComparator;

public class ProductComparatorWidgetItemModifier : IWebPagePanelItemModifier
{
    public WebPagePanelItem Modify(WebPagePanelItem webPagePanelItem,
        WebPagePanelItemModifierParameters webPagePanelItemModifierParameters)
    {
        //TODO 9/26/2023 PavelHess: check and finalize
        webPagePanelItem.SelectableOption.Selectable = true;
        return webPagePanelItem;
    }
}
