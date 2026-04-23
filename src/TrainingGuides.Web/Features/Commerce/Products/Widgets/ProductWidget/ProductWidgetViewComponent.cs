using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Commerce.Products.Services;
using TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductWidget;

[assembly:
    RegisterWidget(
        identifier: ProductWidgetViewComponent.IDENTIFIER,
        viewComponentType: typeof(ProductWidgetViewComponent),
        name: "Product widget",
        propertiesType: typeof(ProductWidgetProperties),
        Description = "Displays product information with variants, images, and optional call-to-action.",
        IconClass = "icon-box")]

namespace TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductWidget;

public class ProductWidgetViewComponent(
    IProductService productService,
    IStringLocalizer<SharedResources> stringLocalizer) : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ProductWidget";

    public async Task<ViewViewComponentResult> InvokeAsync(ProductWidgetProperties properties)
    {
        ProductPage? productPage = null;

        if (properties.DisplayCurrentPage)
        {
            // Retrieve current page with depth 4 to include all nested content
            productPage = await productService.GetCurrentProductPage();
        }
        else
        {
            // Retrieve selected product page
            var selectedProductGuid = properties.SelectedProduct?.FirstOrDefault()?.Identifier;
            if (selectedProductGuid.HasValue && selectedProductGuid.Value != Guid.Empty)
            {
                productPage = await productService.GetProductPageByGuid(selectedProductGuid.Value);
            }
        }

        // Extract the first product from the page
        var product = productPage?.ProductPageProducts?.FirstOrDefault();

        // Get selected variant from querystring if variant selection is enabled
        IProductSchema? selectedVariant = null;
        if (properties.ShowVariantSelection && product is IProductParentSchema parentProduct && productService.ProductHasVariants(product))
        {
            string variantCodeName = HttpContext.Request.Query["variant"].ToString();
            if (!string.IsNullOrWhiteSpace(variantCodeName))
            {
                selectedVariant = productService.GetVariantByCodeName(parentProduct, variantCodeName);
            }
        }

        bool accessDenied = !productService.CanCurrentUserAccessProductPage(productPage, product, selectedVariant);
        string productPageRelativePath = productPage?.GetUrl()?.RelativePath ?? string.Empty;

        var productModel = product is not null
            ? await productService.GetViewModel(product, selectedVariant, accessDenied, productPageRelativePath)
            : null;

        bool productIsSecured = productModel?.IsSecured ?? false;
        bool requiresSignIn = productModel?.RequiresSignIn ?? false;

        // Create view model
        var model = new ProductWidgetViewModel
        {
            Product = productModel,
            ProductPageUrl = productModel?.ProductActionUrl ?? string.Empty,
            ShowVariantSelection = properties.ShowVariantSelection && !productIsSecured,
            ShowVariantDetails = properties.ShowVariantDetails && !productIsSecured,
            ShowCallToAction = (properties.ShowCallToAction && !productIsSecured) || requiresSignIn,
            CallToActionText = productIsSecured
                ? (requiresSignIn ? stringLocalizer["Sign in"] : stringLocalizer["Read more"])
                : properties.CallToActionText ?? stringLocalizer["View Product"]
        };

        return View("~/Features/Commerce/Products/Widgets/ProductWidget/ProductWidget.cshtml", model);
    }
}
