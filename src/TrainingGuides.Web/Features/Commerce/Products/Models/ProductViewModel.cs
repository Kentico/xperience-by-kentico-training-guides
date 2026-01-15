using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Commerce.Products.Models;

public class ProductViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public string ProductDescription { get; set; } = string.Empty;
    public string ProductSkuCode { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public IEnumerable<ProductImageViewModel> ProductImageUrls { get; set; } = Enumerable.Empty<ProductImageViewModel>();
    public string ProductSelectedVariantCodeName { get; set; } = string.Empty;
    public IEnumerable<VariantViewModel> ProductVariants { get; set; } = Enumerable.Empty<VariantViewModel>();
    public HtmlString ProductParentDescription { get; set; } = new HtmlString(string.Empty);
    public HtmlString ProductVariantDescription { get; set; } = new HtmlString(string.Empty);
    public HtmlString ProductOtherDetails { get; set; } = new HtmlString(string.Empty);
    public ProductStockEnum ProductStockStatus { get; set; } = ProductStockEnum.OutOfStock;
}