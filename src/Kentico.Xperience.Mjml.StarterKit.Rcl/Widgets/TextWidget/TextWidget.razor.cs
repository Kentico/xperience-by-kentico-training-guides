using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

using Microsoft.AspNetCore.Components;

[assembly: RegisterEmailWidget(
    identifier: TextWidget.IDENTIFIER,
    name: "{$TextWidget.Name$}",
    componentType: typeof(TextWidget),
    PropertiesType = typeof(TextWidgetProperties),
    IconClass = "icon-l-header-text",
    Description = "{$TextWidget.Description$}"
    )]

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

/// <summary>
/// Text widget component.
/// </summary>
public partial class TextWidget : ComponentBase
{
    private EmailContext? emailContext;

    /// <summary>
    /// The component identifier.
    /// </summary>
    public const string IDENTIFIER = $"Kentico.Xperience.Mjml.StarterKit.{nameof(TextWidget)}";

    /// <summary>
    /// The widget properties.
    /// </summary>
    [Parameter]
    public TextWidgetProperties Properties { get; set; } = null!;

    /// <summary>
    /// Gets or sets the email context accessor service.
    /// </summary>
    [Inject]
    protected IEmailContextAccessor EmailContextAccessor { get; set; } = null!;

    /// <summary>
    /// Gets the current email context.
    /// </summary>
    protected EmailContext EmailContext => emailContext ??= EmailContextAccessor.GetContext();
}
