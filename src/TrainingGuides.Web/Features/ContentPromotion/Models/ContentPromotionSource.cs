namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// Where the promoted content comes from. Stored on the widget, so these values must stay stable.
/// </summary>
public static class ContentPromotionSource
{
    public const string PAGE = "page";
    public const string CONTENT_ITEM = "contentItem";
    public const string MANUAL = "manual";
}
