using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Products.Models;

public class ProductPageViewModel : PageViewModel
{
    public HtmlString Name { get; set; } = HtmlString.Empty;
    public HtmlString ShortDescription { get; set; } = HtmlString.Empty;
    public string Description { get; set; } = string.Empty;
    public List<AssetViewModel> Media { get; set; } = [];
    public decimal Price { get; set; }
    public List<ProductFeatureViewModel> Features { get; set; } = [];
    public List<BenefitViewModel> Benefits { get; set; } = [];
}
