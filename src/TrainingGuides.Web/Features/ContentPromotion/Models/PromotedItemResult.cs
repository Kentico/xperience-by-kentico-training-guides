namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// The outcome of resolving the widget's selection: the normalized item and its family.
/// </summary>
public class PromotedItemResult
{
    public PromotedItemSource? Item { get; set; }

    public ContentFamily Family { get; set; } = ContentFamily.None;

    /// <summary>
    /// True when the editor selected something that could not be loaded - unpublished,
    /// deleted, or not available in the current language. Distinct from having selected
    /// nothing at all, which the widget treats as a valid authoring state.
    /// </summary>
    public bool SelectionFailed { get; set; }

    /// <summary>
    /// The content item the card's values were projected from - the unwrapped item in page
    /// mode, the selected item itself in content hub mode. Kept alongside the normalized
    /// projection so the type-specific extras can read fields no other family has.
    /// </summary>
    public object? PromotedContent { get; set; }

    /// <summary>
    /// The page the selection resolved to, when the selection was a page, so the link rules do
    /// not have to retrieve it again. Null whenever the selection was not a page.
    /// </summary>
    public IWebPageFieldsSource? Page { get; set; }
}
