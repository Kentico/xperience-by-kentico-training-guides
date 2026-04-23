using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Commerce.Products.Models;

public class ProductViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public string ProductSkuCode { get; set; } = string.Empty;
    public decimal ProductRegularPrice { get; set; }
    public decimal ProductPrice { get; set; }
    public IEnumerable<ProductImageViewModel> ProductImages { get; set; } = Enumerable.Empty<ProductImageViewModel>();
    public string ProductSelectedVariantCodeName { get; set; } = string.Empty;
    public IEnumerable<VariantViewModel> ProductVariants { get; set; } = Enumerable.Empty<VariantViewModel>();
    public HtmlString ProductParentDescription { get; set; } = new HtmlString(string.Empty);
    public HtmlString ProductVariantDescription { get; set; } = new HtmlString(string.Empty);
    public HtmlString ProductOtherDetails { get; set; } = new HtmlString(string.Empty);
    public string ProductStockStatus { get; set; } = string.Empty;
    public bool IsSecured { get; set; }
    public bool RequiresSignIn { get; set; }
    public string ProductActionUrl { get; set; } = string.Empty;
}
