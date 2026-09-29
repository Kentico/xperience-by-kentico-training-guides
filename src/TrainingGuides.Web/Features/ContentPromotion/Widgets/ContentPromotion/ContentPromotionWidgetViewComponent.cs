using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.ContentPromotion.Services;
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

public class ContentPromotionWidgetViewComponent(
    IContentPromotionService contentPromotionService) : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ContentPromotionWidget";

    public async Task<ViewViewComponentResult> InvokeAsync(ContentPromotionWidgetProperties properties)
    {
        var promotedItem = await contentPromotionService.ResolvePromotedItem(properties);

        var model = new ContentPromotionWidgetViewModel
        {
            SelectionFailed = promotedItem.SelectionFailed,
            SelectionUnsupported = promotedItem.SelectionUnsupported,
            DisplayValues = contentPromotionService.ResolveDisplayValues(
                properties,
                promotedItem.Item,
                await contentPromotionService.ResolveOverrideImage(properties)),
            Link = await contentPromotionService.ResolveLink(properties, promotedItem.Page),
            Extras = await contentPromotionService.ResolveExtras(properties, promotedItem)
        };

        return View("~/Features/ContentPromotion/Widgets/ContentPromotion/ContentPromotionWidget.cshtml", model);
    }

}
