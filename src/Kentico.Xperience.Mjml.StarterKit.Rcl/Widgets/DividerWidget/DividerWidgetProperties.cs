using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

/// <summary>
/// Configurable properties of the <see cref="ButtonWidget"/>.
/// </summary>
public sealed class DividerWidgetProperties : WidgetPropertiesBase
{
    /// <summary>
    /// The border width.
    /// </summary>
    [NumberInputComponent(
        Label = "{$DividerWidget.BorderWidth.Label$}",
        Order = 1,
        ExplanationText = "{$DividerWidget.BorderWidth.ExplanationText$}")]
    public int BorderWidth { get; set; } = 1;

    /// <summary>
    /// The border color.
    /// </summary>
    [TextInputComponent(
        Label = "{$DividerWidget.BorderColor.Label$}",
        Order = 2,
        ExplanationText = "{$DividerWidget.BorderColor.ExplanationText$}")]
    public string BorderColor { get; set; } = string.Empty;

    /// <summary>
    /// The style of the border.
    /// </summary>
    [TextInputComponent(
        Label = "{$DividerWidget.BorderStyle.Label$}",
        Order = 3,
        ExplanationText = "{$DividerWidget.BorderStyle.ExplanationText$}")]
    public string BorderStyle { get; set; } = string.Empty;
}
