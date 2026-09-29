namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// Why the widget cannot render a complete card. The shared <c>IWidgetViewModel</c> contract
/// only exposes a single bool, which cannot tell this widget's three states apart, so the
/// view model adds this alongside it for the edit-mode notice to render.
/// </summary>
public enum MisconfigurationReason
{
    /// <summary>
    /// The widget is fully configured and renders a card with a destination.
    /// </summary>
    None,

    /// <summary>
    /// No item is selected and no values were typed in - there is nothing to show at all.
    /// </summary>
    NothingAuthored,

    /// <summary>
    /// An item is selected but could not be loaded - unpublished, deleted, or missing in
    /// the current language.
    /// </summary>
    ItemCouldNotBeLoaded,

    /// <summary>
    /// The editor selected a page that loaded fine but holds nothing promotable - a section or
    /// container page, which the selector offers only so the content tree can be navigated.
    /// </summary>
    UnsupportedPageType,

    /// <summary>
    /// The card has content but nowhere to link to. A warning rather than a misconfiguration:
    /// the public still sees a valid card, so only edit mode reacts to this.
    /// </summary>
    NoDestination
}
