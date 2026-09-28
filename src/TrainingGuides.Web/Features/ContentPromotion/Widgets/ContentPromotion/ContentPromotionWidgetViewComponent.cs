using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;

[assembly:
    RegisterWidget(
        identifier: ContentPromotionWidgetViewComponent.IDENTIFIER,
        viewComponentType: typeof(ContentPromotionWidgetViewComponent),
        name: "Content promotion",
        propertiesType: typeof(ContentPromotionWidgetProperties),
        Description = "Promotes a single article, product or service as a card, with every displayed value overridable per widget instance.",
        IconClass = "icon-megaphone")]

namespace TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;

public class ContentPromotionWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ContentPromotionWidget";

    public ViewViewComponentResult Invoke(ContentPromotionWidgetProperties properties)
    {
        var model = new ContentPromotionWidgetViewModel();

        return View("~/Features/ContentPromotion/Widgets/ContentPromotion/ContentPromotionWidget.cshtml", model);
    }
}
