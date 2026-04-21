using CMS.ContentEngine;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Commerce.Products.Services;
using TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductWidget;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Shared.Helpers;

[assembly:
    RegisterWidget(
        identifier: ProductWidgetViewComponent.IDENTIFIER,
        viewComponentType: typeof(ProductWidgetViewComponent),
        name: "Product widget",
        propertiesType: typeof(ProductWidgetProperties),
        Description = "Displays product information with variants, images, and optional call-to-action.",
        IconClass = "icon-box")]

namespace TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductWidget;

public class ProductWidgetViewComponent(IProductService productService,
        IMembershipService membershipService,
        IPreferredLanguageRetriever preferredLanguageRetriever) : ViewComponent
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

        var user = HttpContext?.User;
        bool pageHasAccess = productPage?.HasAccess(user) ?? true;
        bool itemHasAccess = (product as IContentItemFieldsSource)?.HasAccess(user) ?? true;

        bool accessDenied = !pageHasAccess || !itemHasAccess;

        string language = preferredLanguageRetriever.Get();
        bool isAuthenticated = await membershipService.IsMemberAuthenticated();

        string deniedReturnPath = (productPage?.GetUrl()?.RelativePath ?? string.Empty).TrimStart('~');
        string accessDeniedUrl = $"{ApplicationConstants.ACCESS_DENIED_ACTION_PATH}{QueryString.Create(ApplicationConstants.RETURN_URL_PARAMETER, deniedReturnPath)}";

        // Create view model
        var model = new ProductWidgetViewModel
        {
            Product = product is not null
                ? await productService.GetViewModel(product, selectedVariant, accessDenied)
                : null,
            ProductPageUrl = accessDenied
                ? isAuthenticated
                    ? accessDeniedUrl
                    : await membershipService.GetSignInUrl(language, true)
                : productPage?.GetUrl()?.RelativePath ?? string.Empty,
            ShowVariantSelection = properties.ShowVariantSelection && !accessDenied,
            ShowVariantDetails = properties.ShowVariantDetails && !accessDenied,
            ShowCallToAction = properties.ShowCallToAction || accessDenied,
            CallToActionText = accessDenied
                ? isAuthenticated ? "Access denied" : "Sign in"
                : properties.CallToActionText ?? "View Product"
        };

        return View("~/Features/Commerce/Products/Widgets/ProductWidget/ProductWidget.cshtml", model);
    }
}
