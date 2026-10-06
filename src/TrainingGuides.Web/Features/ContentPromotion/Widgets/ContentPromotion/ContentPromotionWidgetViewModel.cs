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
    /// The read-only facts specific to the promoted item's content family. Always present,
    /// and empty whenever there is nothing to show - extras never affect whether the card
    /// itself is considered configured.
    /// </summary>
    public PromotionExtrasViewModel Extras { get; set; } = new();

    /// <summary>
    /// Every class on the card's outer element - base, design, color scheme, corner style and
    /// column layout, already resolved. Computed in the view component so the view stays dumb.
    /// </summary>
    public string CardCssClasses { get; set; } = string.Empty;

    /// <summary>
    /// Classes on the text column, carrying the text alignment.
    /// </summary>
    public string ContentCssClasses { get; set; } = string.Empty;

    /// <summary>
    /// Classes on the card image.
    /// </summary>
    public string ImageCssClasses { get; set; } = string.Empty;

    /// <summary>
    /// Classes on the call to action anchor, carrying the chosen link style.
    /// </summary>
    public string CallToActionCssClasses { get; set; } = string.Empty;

    /// <summary>
    /// The raw corner style value, passed through for the <c>tg-styled-image</c> tag helper.
    /// </summary>
    public string CornerStyle { get; set; } = string.Empty;

    /// <summary>
    /// The value logged as the click activity, or empty when the editor set none.
    /// </summary>
    public string TrackingValue { get; set; } = string.Empty;

    /// <summary>
    /// True when the editor selected a page that loaded but holds nothing promotable.
    /// </summary>
    public bool SelectionUnsupported { get; set; }

    /// <summary>
    /// True when the editor selected an item that could not be loaded. Distinct from having
    /// selected nothing, which is a valid authoring state in manual mode.
    /// </summary>
    public bool SelectionFailed { get; set; }

    /// <summary>
    /// True when the editor hid the call to action. The resolved text is empty either way, so
    /// this is what tells a deliberate hide apart from text that was never provided.
    /// </summary>
    public bool CallToActionHidden { get; set; }

    public MisconfigurationReason MisconfigurationReason
    {
        get
        {
            if (SelectionUnsupported)
            {
                return MisconfigurationReason.UnsupportedPageType;
            }

            if (SelectionFailed)
            {
                return MisconfigurationReason.ItemCouldNotBeLoaded;
            }

            if (!DisplayValues.HasContent)
            {
                return MisconfigurationReason.NothingAuthored;
            }

            if (Link is null)
            {
                return MisconfigurationReason.NoDestination;
            }

            if (CallToActionHidden)
            {
                return MisconfigurationReason.CallToActionHidden;
            }

            return string.IsNullOrWhiteSpace(DisplayValues.CallToActionText)
                ? MisconfigurationReason.CallToActionMissing
                : MisconfigurationReason.None;
        }
    }

    /// <summary>
    /// A card with content but no destination is deliberately not misconfigured - the public
    /// still sees a valid card, and only the edit-mode notice reacts to the missing link.
    /// </summary>
    public bool IsMisconfigured => MisconfigurationReason
        is MisconfigurationReason.NothingAuthored
        or MisconfigurationReason.ItemCouldNotBeLoaded
        or MisconfigurationReason.UnsupportedPageType;
}
