namespace TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductListing;

public class ProductListingFilterViewModel
{
    public string ProductListingFilterName { get; set; } = string.Empty;
    public string ProductListingFilterDisplayName { get; set; } = string.Empty;
    public List<ProductListingFilterOptionViewModel> ProductListingFilterOptions { get; set; } = [];
}

public class ProductListingFilterOptionViewModel
{
    public string FilterOptionDisplayName { get; set; } = string.Empty;
    public string FilterOptionValue { get; set; } = string.Empty;
}