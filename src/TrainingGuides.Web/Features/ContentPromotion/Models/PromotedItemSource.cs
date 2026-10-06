using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// The values read from the selected content item, normalized across content families.
/// Null when the widget is in manual mode or nothing could be resolved.
/// </summary>
public class PromotedItemSource
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public AssetViewModel? Image { get; set; }
}
