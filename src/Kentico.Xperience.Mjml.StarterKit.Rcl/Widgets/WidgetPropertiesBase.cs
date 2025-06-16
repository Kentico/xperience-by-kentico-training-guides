using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

/// <summary>
/// Base class for all widget properties that provides common properties.
/// This class serves as a foundation for all widget property classes, ensuring
/// consistent property definitions across different widget types.
/// </summary>
public abstract class WidgetPropertiesBase : IEmailWidgetProperties
{
    /// <summary>
    /// The CSS class for this widget
    /// </summary>
    [TextInputComponent(
        Label = "{$WidgetPropertiesBase.CssClass.Label$}",
        Order = 100,
        ExplanationText = "{$WidgetPropertiesBase.CssClass.ExplanationText$}")]
    public string CssClass { get; set; } = string.Empty;
}
