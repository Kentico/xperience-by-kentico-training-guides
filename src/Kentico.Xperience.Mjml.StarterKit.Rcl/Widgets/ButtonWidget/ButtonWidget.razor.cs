using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

using Microsoft.AspNetCore.Components;

[assembly: RegisterEmailWidget(
    identifier: ButtonWidget.IDENTIFIER,
    name: "{$ButtonWidget.Name$}",
    componentType: typeof(ButtonWidget),
    PropertiesType = typeof(ButtonWidgetProperties),
    IconClass = "icon-arrow-right-top-square",
    Description = "{$ButtonWidget.Description$}"
    )]

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

/// <summary>
/// Button widget component.
/// </summary>
public partial class ButtonWidget : ComponentBase
{
    /// <summary>
    /// The component identifier.
    /// </summary>
    public const string IDENTIFIER = $"Kentico.Xperience.Mjml.StarterKit.{nameof(ButtonWidget)}";

    /// <summary>
    /// The widget properties.
    /// </summary>
    [Parameter]
    public ButtonWidgetProperties Properties { get; set; } = null!;
}
