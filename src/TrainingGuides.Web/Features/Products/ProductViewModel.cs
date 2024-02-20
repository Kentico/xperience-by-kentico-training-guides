using TrainingGuides.Web.Features.Shared.Models;
using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.Products;

public class ProductViewModel : PageViewModel
{
    public HtmlString Name { get; set; } = null!;
    public string Header { get; set; } = null!;
    public HtmlString ShortDescription { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<AssetViewModel?> Media { get; set; } = [];
    public LinkViewModel? Link { get; set; }
    public string CallToAction { get; set; } = null!;
    public decimal Price { get; set; }
    public List<ProductFeaturesViewModel> Features { get; set; } = [];
}
