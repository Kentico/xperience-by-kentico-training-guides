using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

using Microsoft.AspNetCore.Components;

[assembly: RegisterEmailWidget(
    identifier: ImageWidget.IDENTIFIER,
    name: "{$ImageWidget.Name$}",
    componentType: typeof(ImageWidget),
    PropertiesType = typeof(ImageWidgetProperties),
    IconClass = "icon-picture",
    Description = "{$ImageWidget.Description$}"
    )]

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

/// <summary>
/// Image widget component.
/// </summary>
public partial class ImageWidget : ComponentBase
{
    [Inject]
    private IComponentModelMapper<ImageWidgetModel> ImageComponentModelMapper { get; set; } = default!;

    [Inject]
    private IEmailContextAccessor EmailContextAccessor { get; set; } = default!;

    /// <summary>
    /// The component identifier.
    /// </summary>
    public const string IDENTIFIER = $"Kentico.Xperience.Mjml.StarterKit.{nameof(ImageWidget)}";

    /// <summary>
    /// The widget model.
    /// </summary>
    public ImageWidgetModel Model { get; set; } = new();

    /// <summary>
    /// The widget properties.
    /// </summary>
    [Parameter]
    public ImageWidgetProperties Properties { get; set; } = null!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        var selectedId = Properties.Assets?.Select(i => i.Identifier).FirstOrDefault();

        if (selectedId is null)
        {
            return;
        }

        var languageName = EmailContextAccessor.GetContext().LanguageName;

        Model = await ImageComponentModelMapper.Map(selectedId.Value, languageName);
    }
}
