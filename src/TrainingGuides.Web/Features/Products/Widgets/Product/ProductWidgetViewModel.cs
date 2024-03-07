using TrainingGuides.Web.Features.Products.Models;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Products.Widgets.Product;

public class ProductWidgetViewModel
{
    public ProductPageViewModel Product { get; set; } = null!;
    public bool ShowProductFeatures { get; set; }
    public AssetViewModel? ProductImage { get; set; }
    public bool ShowAdvanced { get; set; }
    public string? ParentElementCssClasses { get; set; }
    public string? MainContentElementCssClasses { get; set; }
    public string? ImageElementCssClasses { get; set; }
    public string? ColorScheme { get; set; }
    public string? CornerStyle { get; set; }
    public string? CallToActionCssClasses { get; set; }


    // can be extracted into a parent class and have ProductWidgetViewModel inherit from it
    // see for example Product comparator or Hero banner widget view model
    public bool IsMisconfigured => Product == null;
}
