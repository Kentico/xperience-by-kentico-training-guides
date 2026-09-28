using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;

public class ContentPromotionWidgetViewModel : IWidgetViewModel
{
    /// <summary>
    /// The values the card renders, after hide, override and inherit have been applied.
    /// </summary>
    public ContentPromotionDisplayValues DisplayValues { get; set; } = new();

    /// <summary>
    /// The card's destination, or null when the widget has none.
    /// </summary>
    public LinkViewModel? Link { get; set; }

    /// <summary>
    /// True when the editor selected an item that could not be loaded. Distinct from having
    /// selected nothing, which is a valid authoring state in manual mode.
    /// </summary>
    public bool SelectionFailed { get; set; }

    public MisconfigurationReason MisconfigurationReason
    {
        get
        {
            if (SelectionFailed)
            {
                return MisconfigurationReason.ItemCouldNotBeLoaded;
            }

            if (!DisplayValues.HasContent)
            {
                return MisconfigurationReason.NothingAuthored;
            }

            return Link is null
                ? MisconfigurationReason.NoDestination
                : MisconfigurationReason.None;
        }
    }

    /// <summary>
    /// A card with content but no destination is deliberately not misconfigured - the public
    /// still sees a valid card, and only the edit-mode notice reacts to the missing link.
    /// </summary>
    public bool IsMisconfigured => MisconfigurationReason
        is MisconfigurationReason.NothingAuthored
        or MisconfigurationReason.ItemCouldNotBeLoaded;
}
