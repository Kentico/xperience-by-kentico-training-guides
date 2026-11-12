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
    /// The description as markup: inherited descriptions are rich text. A typed override arrives
    /// already encoded, because it comes from a plain text area.
    /// </summary>
    public HtmlString DescriptionHtml { get; set; } = HtmlString.Empty;

    public string CallToActionText { get; set; } = string.Empty;

    public AssetViewModel? Image { get; set; }

    /// <summary>
    /// True when at least one element is left to render.
    /// </summary>
    public bool HasContent =>
        !string.IsNullOrWhiteSpace(Title)
        || !string.IsNullOrWhiteSpace(DescriptionHtml.Value)
        || !string.IsNullOrWhiteSpace(CallToActionText)
        || !string.IsNullOrWhiteSpace(Image?.FilePath);
}
