using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Products.Models;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.OptionProviders.Heading;

namespace TrainingGuides.Web.Features.Products.Widgets.ProductComparator;

public class ProductComparatorWidgetViewModel : WidgetViewModel
{
    public List<ProductPageViewModel> Products { get; set; } = [];
    public List<KeyValuePair<string, HtmlString>> GroupedFeatures { get; set; } = [];
    public string ComparatorHeading { get; set; } = null!;
    public HeadingTypeOption HeadingType { get; set; }
    public string HeadingMargin { get; set; }
    public bool ShowShortDescription { get; set; }
    public string? CheckboxIconUrl { get; set; }
    public override bool IsMisconfigured => Products.Count == 0;
}
