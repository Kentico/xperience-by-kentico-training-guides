using TrainingGuides.Web.Features.Products.Models;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Products.Widgets.Product;

public class ProductWidgetViewModel : IWidgetViewModel
{
    public ProductPageViewModel? Product { get; set; }
    public bool ShowProductFeatures { get; set; }
    public AssetViewModel ProductImage { get; set; } = new();
    public bool ShowAdvanced { get; set; }
    public string ParentElementCssClasses { get; set; } = string.Empty;
    public string MainContentElementCssClasses { get; set; } = string.Empty;
    public string ImageElementCssClasses { get; set; } = string.Empty;
    public string ColorScheme { get; set; } = string.Empty;
    public string CornerStyle { get; set; } = string.Empty;
    public string CallToActionCssClasses { get; set; } = string.Empty;
    public bool IsImagePositionSide { get; set; } = false;

    public bool IsMisconfigured => Product == null;
}
