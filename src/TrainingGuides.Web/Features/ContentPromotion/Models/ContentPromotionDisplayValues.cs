using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// The values the card actually renders, after applying hide, override and inherit.
/// </summary>
public class ContentPromotionDisplayValues
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string CallToActionText { get; set; } = string.Empty;

    public AssetViewModel? Image { get; set; }
}
