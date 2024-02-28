using TrainingGuides.Web.Features.Products.Widgets.Product;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

[assembly: RegisterWidget(
    ProductWidgetViewComponent.IDENTIFIER,
    typeof(ProductWidgetViewComponent),
    "Product",
    typeof(ProductWidgetProperties),
    Description = "Displays the product.",
    IconClass = "icon-ribbon")]

namespace TrainingGuides.Web.Features.Products.Widgets.Product;

public class ProductWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "Kentico.KBank.ProductWidget";

    public ProductWidgetViewComponent()
    {
    }

    public async Task<ViewViewComponentResult> InvokeAsync(ProductWidgetProperties properties)
    {
        var model = new ProductWidgetViewModel()
        {
        };

        return View("~/Features/Products/Widgets/Product/ProductWidget.cshtml", model);
    }
}