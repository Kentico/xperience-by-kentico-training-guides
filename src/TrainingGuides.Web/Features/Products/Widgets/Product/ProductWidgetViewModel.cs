using TrainingGuides.Web.Features.Products.Models;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Products.Widgets.Product;

public class ProductWidgetViewModel : WidgetViewModel
{
    public ProductPageViewModel? Product { get; set; }
    public bool ShowProductFeatures { get; set; }
    public AssetViewModel? ProductImage { get; set; }
    public bool ShowAdvanced { get; set; }
    public string? ParentElementCssClasses { get; set; }
    public string? MainContentElementCssClasses { get; set; }
    public string? ImageElementCssClasses { get; set; }
    public string? ColorScheme { get; set; }
    public string? CornerStyle { get; set; }
    public string? CallToActionCssClasses { get; set; }
    public bool IsImagePositionSide { get; set; } = false;

    public override bool IsMisconfigured => Product == null;
}
