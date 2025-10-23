using CMS.Core;

using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

[assembly: RegisterEmailWidget(
    identifier: ProductWidget.IDENTIFIER,
    name: "{$ProductWidget.Name$}",
    componentType: typeof(ProductWidget),
    PropertiesType = typeof(ProductWidgetProperties),
    IconClass = "icon-t-shirt",
    Description = "{$ProductWidget.Description$}"
    )]

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

/// <summary>
/// Product widget component.
/// </summary>
public partial class ProductWidget : ComponentBase
{
    /// <summary>
    /// The component identifier.
    /// </summary>
    public const string IDENTIFIER = $"Kentico.Xperience.Mjml.StarterKit.{nameof(ProductWidget)}";

    [Inject]
    private IComponentModelMapper<ProductWidgetModel> ProductComponentModelMapper { get; set; } = default!;

    [Inject]
    private IEmailContextAccessor EmailContextAccessor { get; set; } = default!;

    [Inject]
    private ILogger<ProductWidget> Logger { get; set; } = default!;

    /// <summary>
    /// The widget model.
    /// </summary>
    public ProductWidgetModel Model { get; set; } = new();

    /// <summary>
    /// The widget properties.
    /// </summary>
    [Parameter]
    public ProductWidgetProperties Properties { get; set; } = new();

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        var webPageItem = Properties.Pages.FirstOrDefault();

        if (webPageItem is null)
        {
            return;
        }

        string languageName = EmailContextAccessor.GetContext().LanguageName;

        Model = await ProductComponentModelMapper.Map(webPageItem.Identifier, languageName);

        if (Model is null)
        {
            Logger.LogError("An attempt to use the {ProductWidget} email builder widget component has been made, but the {Mapper} has not been registered.", nameof(ProductWidget), nameof(IComponentModelMapper<ProductWidget>));
        }
    }
}
