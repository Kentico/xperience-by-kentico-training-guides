using System.ComponentModel;

namespace TrainingGuides.Web.Features.ContentPromotion.Models;

/// <summary>
/// The card layouts the promotion widget offers. Stored on the widget by member name, so the
/// names must stay stable.
/// </summary>
public enum CardDesignOption
{
    [Description("Standard card")]
    Standard = 0,

    /// <summary>
    /// Text sits on top of the image, which becomes the card background. Paints its own
    /// background, so the color scheme property does not apply.
    /// </summary>
    [Description("Image overlay")]
    ImageOverlay = 1,

    /// <summary>
    /// A brand gradient behind the text. Paints its own background, so the color scheme
    /// property does not apply.
    /// </summary>
    [Description("Gradient")]
    Gradient = 2,

    /// <summary>
    /// An oversized, centered card for a single prominent promotion.
    /// </summary>
    [Description("Spotlight")]
    Spotlight = 3,

    /// <summary>
    /// A tall card with the image locked to a portrait aspect ratio.
    /// </summary>
    [Description("Portrait")]
    Portrait = 4
}
