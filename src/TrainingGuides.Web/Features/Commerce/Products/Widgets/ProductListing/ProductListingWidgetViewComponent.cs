using CMS.ContentEngine;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Commerce.Products.Services;
using TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductListing;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(
        identifier: ProductListingWidgetViewComponent.IDENTIFIER,
        viewComponentType: typeof(ProductListingWidgetViewComponent),
        name: "Product listing widget",
        propertiesType: typeof(ProductListingWidgetProperties),
        Description = "Displays a list of products from a selected content tree section.",
        IconClass = "icon-list")]

namespace TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductListing;

public class ProductListingWidgetViewComponent(
        IContentItemRetrieverService contentItemRetrieverService,
        IProductService productService,
        IWebPageDataContextRetriever webPageDataContextRetriever) : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ProductListingWidget";

    public async Task<ViewViewComponentResult> InvokeAsync(ProductListingWidgetProperties properties)
    {
        var model = new ProductListingWidgetViewModel
        {
            CtaText = properties.CtaText,
            SignInText = properties.SignInText
        };

        string parentPagePath = await GetParentPagePath(properties.ParentPageSelection);

        if (!string.IsNullOrEmpty(parentPagePath))
        {
            var productPages = await productService.RetrieveProductPages(
                parentPagePath,
                properties.SecuredItemsDisplayMode);

            model.Products = await productService.GetProductListingItemViewModels(
                productPages,
                properties.SecuredItemsDisplayMode);
        }

        return View("~/Features/Commerce/Products/Widgets/ProductListing/ProductListingWidget.cshtml", model);
    }

    private async Task<string> GetParentPagePath(IEnumerable<ContentItemReference> parentPageSelection)
    {
        // If a specific page is selected, use it
        if (parentPageSelection.Any())
        {
            var selectedPageGuid = parentPageSelection.First().Identifier;
            var selectedPage = await contentItemRetrieverService.RetrieveWebPageByContentItemGuid(selectedPageGuid);
            return selectedPage?.SystemFields.WebPageItemTreePath ?? string.Empty;
        }

        // Otherwise, use the current page from the web page context
        var currentPageContext = webPageDataContextRetriever.Retrieve();
        if (currentPageContext.WebPage != null)
        {
            var currentPage = await contentItemRetrieverService.RetrieveWebPageById(
                currentPageContext.WebPage.WebPageItemID);
            return currentPage?.SystemFields.WebPageItemTreePath ?? string.Empty;
        }

        return string.Empty;
    }
}
