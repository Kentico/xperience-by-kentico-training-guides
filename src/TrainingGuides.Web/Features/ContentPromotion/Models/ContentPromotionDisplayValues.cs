using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// The values the card actually renders, after applying hide, override and inherit.
/// </summary>
public class ContentPromotionDisplayValues
{
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description as markup. Every content family stores its description as rich text and
    /// every other consumer in the project renders it with <see cref="HtmlString"/>, so the card
    /// does the same - rendering it as a plain string shows the editor's tags on screen. The
    /// typed override arrives here already encoded, because its form component is a plain text
    /// area and an author typing a stray angle bracket is not writing markup.
    /// </summary>
    public HtmlString DescriptionHtml { get; set; } = HtmlString.Empty;

    public string CallToActionText { get; set; } = string.Empty;

    public AssetViewModel? Image { get; set; }

    /// <summary>
    /// True when at least one element survives to be rendered. Everything hidden, or nothing
    /// resolved and nothing typed in, leaves the card with nothing to show.
    /// </summary>
    public bool HasContent =>
        !string.IsNullOrWhiteSpace(Title)
        || !string.IsNullOrWhiteSpace(DescriptionHtml.Value)
        || !string.IsNullOrWhiteSpace(CallToActionText)
        || !string.IsNullOrWhiteSpace(Image?.FilePath);
}
