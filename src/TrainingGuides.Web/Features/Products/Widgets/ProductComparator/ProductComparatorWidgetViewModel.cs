using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Products.Models;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Products.Widgets.ProductComparator;

public class ProductComparatorWidgetViewModel : IWidgetViewModel
{
    public List<ProductPageViewModel> Products { get; set; } = [];
    public List<KeyValuePair<string, HtmlString>> GroupedFeatures { get; set; } = [];
    public string ComparatorHeading { get; set; } = string.Empty;
    public string HeadingType { get; set; } = string.Empty;
    public string HeadingMargin { get; set; } = string.Empty;
    public bool ShowShortDescription { get; set; }
    public string CheckboxIconUrl { get; set; } = string.Empty;
    public bool IsMisconfigured => Products.Count == 0;
}
