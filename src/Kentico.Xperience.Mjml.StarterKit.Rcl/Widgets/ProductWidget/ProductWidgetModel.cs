namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

/// <summary>
/// The product widget model.
/// </summary>
public sealed class ProductWidgetModel
{
    /// <summary>
    /// The product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The product description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The product image URL.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// The product image alternate text.
    /// </summary>
    public string? ImageAltText { get; set; }

    /// <summary>
    /// The Web Page Item URL which the widget is mapped to.
    /// </summary>
    public string Url { get; set; } = string.Empty;
}
