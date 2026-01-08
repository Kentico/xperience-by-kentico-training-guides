using Kentico.Xperience.Admin.Websites;

namespace TrainingGuides.Web.Features.FinancialServices.Widgets.ServiceComparator;

public class ServiceComparatorWidgetItemModifier : IWebPagePanelItemModifier
{
    public WebPagePanelItem Modify(WebPagePanelItem webPagePanelItem,
        WebPagePanelItemModifierParameters webPagePanelItemModifierParameters)
    {
        webPagePanelItem.SelectableOption.Selectable = true;
        return webPagePanelItem;
    }
}
