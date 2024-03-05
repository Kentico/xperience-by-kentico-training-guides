using TrainingGuides.Web.Features.Shared.Models;
using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.Products.Models;

public class ProductPageViewModel : PageViewModel
{
    public HtmlString? Name { get; set; }
    public HtmlString? ShortDescription { get; set; }
    public string? Description { get; set; }
    public List<AssetViewModel> Media { get; set; } = [];
    public LinkViewModel? Link { get; set; }
    public decimal Price { get; set; }
    public List<ProductFeaturesViewModel> Features { get; set; } = [];
    public List<BenefitViewModel> Benefits { get; set; } = [];
}
