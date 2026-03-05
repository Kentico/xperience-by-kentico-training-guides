using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductListing;

public class ProductListingWidgetViewModel : IWidgetViewModel
{
    public List<ProductListingItemViewModel> Products { get; set; } = [];
    public string CtaText { get; set; } = string.Empty;
    public string SignInText { get; set; } = string.Empty;
    public bool IsMisconfigured => Products == null;
}