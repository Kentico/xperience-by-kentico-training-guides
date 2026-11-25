namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// Names of the card elements an editor can suppress with the Hide elements property.
/// Stored on the widget, so these values must stay stable.
/// </summary>
public static class ContentPromotionElement
{
    public const string TITLE = "title";
    public const string DESCRIPTION = "description";
    public const string IMAGE = "image";
    public const string CALL_TO_ACTION = "callToAction";
}
