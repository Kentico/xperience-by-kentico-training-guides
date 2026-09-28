namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// The content family of the promoted item, worked out at runtime from the resolved item
/// rather than chosen by the editor.
/// </summary>
public enum ContentFamily
{
    None,
    Article,
    Product,
    Service
}
