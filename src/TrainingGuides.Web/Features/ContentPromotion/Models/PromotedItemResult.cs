namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// The outcome of resolving the widget's selection: the normalized item and where it came from.
/// </summary>
public class PromotedItemResult
{
    public PromotedItemSource? Item { get; set; }

    /// <summary>
    /// True when the editor selected something that could not be loaded - unpublished, deleted, or
    /// not available in the current language. Selecting nothing is not a failure.
    /// </summary>
    public bool SelectionFailed { get; set; }

    /// <summary>
    /// The content item the card's values came from - the page's linked item in page mode, the
    /// selected item in content hub mode. The extras read type-specific fields from it.
    /// </summary>
    public object? PromotedContent { get; set; }

    /// <summary>
    /// The selected page, so the link rules do not retrieve it again. Null outside page mode.
    /// </summary>
    public IWebPageFieldsSource? Page { get; set; }
}
