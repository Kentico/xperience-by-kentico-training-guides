using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets.Enums;

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

/// <summary>
/// Configurable properties of the <see cref="ButtonWidget"/>.
/// </summary>
public sealed class ButtonWidgetProperties : WidgetPropertiesBase
{
    /// <summary>
    /// The button text.
    /// </summary>
    [TextInputComponent(
        Label = "{$ButtonWidget.Text.Label$}",
        Order = 1,
        ExplanationText = "{$ButtonWidget.Text.ExplanationText$}")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The URL linked by button.
    /// </summary>
    [TextInputComponent(
        Label = "{$ButtonWidget.Url.Label$}",
        Order = 2,
        ExplanationText = "{$ButtonWidget.Url.ExplanationText$}")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The button HTML element type. <see cref="ButtonType"/> 
    /// </summary>
    [DropDownComponent(
        Label = "{$ButtonWidget.ButtonType.Label$}",
        Order = 3,
        ExplanationText = "{$ButtonWidget.ButtonType.ExplanationText$}",
        Options = $"{nameof(Enums.ButtonType.Button)};{{$ButtonType.Button$}}\r\n{nameof(Enums.ButtonType.Link)};{{$ButtonType.Link$}}",
        OptionsValueSeparator = ";")]
    public string ButtonType { get; set; } = nameof(Enums.ButtonType.Button);

    /// <summary>
    /// The horizontal alignment of the button. <see cref="HorizontalAlignment"/>
    /// </summary>
    [DropDownComponent(
        Label = "{$ButtonWidget.Alignment.Label$}",
        Order = 4,
        ExplanationText = "{$ButtonWidget.Alignment.ExplanationText$}",
        Options = $"{nameof(HorizontalAlignment.Left)};{{$HorizontalAlignment.Left$}}\r\n{nameof(HorizontalAlignment.Center)};{{$HorizontalAlignment.Center$}}\r\n{nameof(HorizontalAlignment.Right)};{{$HorizontalAlignment.Right$}}",
        OptionsValueSeparator = ";")]
    public string ButtonHorizontalAlignment { get; set; } = nameof(HorizontalAlignment.Left);
}
