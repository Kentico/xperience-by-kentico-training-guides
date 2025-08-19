using CMS.ContentEngine;

using Kentico.EmailBuilder.Web.Mvc;

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

/// <summary>
/// Configurable properties of the <see cref="TextWidget"/>.
/// </summary>
public sealed class TextWidgetProperties : WidgetPropertiesBase
{
    /// <summary>
    /// The widget content.
    /// </summary>
    [TrackContentItemReference(typeof(ContentItemReferenceExtractor))]
    public string Text { get; set; } = string.Empty;
}
