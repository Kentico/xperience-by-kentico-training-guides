using CMS.ContentEngine;

using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets.Enums;

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

/// <summary>
/// Configurable properties of the <see cref="ImageWidget"/>.
/// </summary>
public sealed class ImageWidgetProperties : WidgetPropertiesBase
{
    /// <summary>
    /// The image.
    /// </summary>
    [ContentItemSelectorComponent(
        typeof(ImageContentTypesFilter),
        Order = 1,
        Label = "{$ImageWidget.Image.Label$}",
        ExplanationText = "{$ImageWidget.Image.ExplanationText$}",
        MaximumItems = 1)]
    public IEnumerable<ContentItemReference> Assets { get; set; } = [];

    /// <summary>
    /// The horizontal alignment of the button. <see cref="HorizontalAlignment"/>
    /// </summary>
    [DropDownComponent(
        Label = "{$ImageWidget.Alignment.Label$}",
        Order = 2,
        ExplanationText = "{$ImageWidget.Alignment.ExplanationText$}",
        Options = $"{nameof(HorizontalAlignment.Left)};{{$HorizontalAlignment.Left$}}\r\n{nameof(HorizontalAlignment.Center)};{{$HorizontalAlignment.Center$}}\r\n{nameof(HorizontalAlignment.Right)};{{$HorizontalAlignment.Right$}}",
        OptionsValueSeparator = ";")]
    public string Alignment { get; set; } = nameof(HorizontalAlignment.Left);

    /// <summary>
    /// The image width.
    /// </summary>
    [NumberInputComponent(
        Label = "{$ImageWidget.Width.Label$}",
        Order = 3,
        ExplanationText = "{$ImageWidget.Width.ExplanationText$}")]
    public int? Width { get; set; }
}
