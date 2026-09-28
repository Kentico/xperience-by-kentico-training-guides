using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.ContentPromotion.Services;

public interface IContentPromotionService
{
    /// <summary>
    /// Applies the hide, override and inherit rules to produce the values the card renders.
    /// </summary>
    ContentPromotionDisplayValues ResolveDisplayValues(
        ContentPromotionWidgetProperties properties,
        PromotedItemSource? item,
        AssetViewModel? overrideImage = null);

    /// <summary>
    /// Resolves the widget's selection into a normalized item and its content family.
    /// </summary>
    Task<PromotedItemResult> ResolvePromotedItem(ContentPromotionWidgetProperties properties);

    /// <summary>
    /// Builds the card's destination. Returns null when the widget has no destination at all,
    /// so the caller never has to interpret an empty URL string.
    /// </summary>
    Task<LinkViewModel?> ResolveLink(
        ContentPromotionWidgetProperties properties,
        IWebPageFieldsSource? selectedPage);
}
