using TrainingGuides.Web.Commerce.Products.Models;

namespace TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductListing;

/// <summary>
/// Wraps ProductViewModel with additional page-level data like URL.
/// </summary>
public class ProductListingItemViewModel
{
    public ProductViewModel Product { get; set; } = new();
    public string ProductPageUrl { get; set; } = string.Empty;
}
