using TrainingGuides.Web.Features.Products.Widgets.Product;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

[assembly: RegisterWidget(
    identifier: ProductWidgetViewComponent.IDENTIFIER,
    viewComponentType: typeof(ProductWidgetViewComponent),
    name: "Product",
    propertiesType: typeof(ProductWidgetProperties),
    Description = "Displays selected product.",
    IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Products.Widgets.Product;

public class ProductWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ProductWidget";

    public ProductWidgetViewComponent()
    {
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ProductWidgetProperties properties)
    {
        // var guid = properties.Product?.Select(i => i.NodeGuid).FirstOrDefault();
        // var product = guid.HasValue ? await GetProduct(guid.Value, properties, cancellationToken) : null;

        var model = new ProductWidgetViewModel()
        {
            Product = null,
            ShowProductFeatures = properties.ShowProductFeatures,
            ShowProductImage = properties.ShowProductImage,
            OpenProductPageOnClick = properties.OpenProductPageOnClick,
            CallToAction = properties.CallToAction,
            OpenInNewTab = properties.OpenInNewTab,
            ShowAdvanced = properties.ShowAdvanced,
            CardSize = properties.CardSize,
            ColorScheme = properties.ColorScheme,
            CornerStyle = properties.CornerStyle,
            ContentAlignment = properties.ContentAlignment
        };

        return View("~/Features/Products/Widgets/Product/ProductWidget.cshtml", model);
    }
}