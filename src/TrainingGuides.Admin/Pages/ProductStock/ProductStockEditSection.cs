using CMS.DataEngine;
using Kentico.Xperience.Admin.Base;
using TrainingGuides.Admin.ProductStock;
using TrainingGuides.ProductStock;

// Registers this as a parameterized edit section that handles routing for individual stock records
[assembly: UIPage(typeof(ProductStockList), PageParameterConstants.PARAMETERIZED_SLUG, typeof(ProductStockEditSection), "Product stock section caption", TemplateNames.SECTION_LAYOUT, UIPageOrder.NoOrder)]

namespace TrainingGuides.Admin.ProductStock;

// Edit section that handles URL parameterization and routing for editing specific product stock records.
// Acts as a container/entry point for the edit operation.
public sealed class ProductStockEditSection(IProductMetadataRetriever productMetadataRetriever) : EditSectionPage<ProductAvailableStockInfo>
{

    // Gets the display name for the object being edited (used in breadcrumbs and page titles)
    protected override async Task<string> GetObjectDisplayName(BaseInfo infoObject)
    {
        // Retrieves the product name for display in the edit section header
        // This ensures the page shows "Edit Product Stock - [Product Name]" instead of just an ID
        var product = await productMetadataRetriever.GetProductMetadata(infoObject as ProductAvailableStockInfo ?? new());
        return product.DisplayName;
    }

}