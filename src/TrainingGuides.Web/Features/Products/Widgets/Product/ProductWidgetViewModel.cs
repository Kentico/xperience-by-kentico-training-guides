using TrainingGuides.Web.Features.Products.Models;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Products.Widgets.Product;

public class ProductWidgetViewModel
{
    public ProductPageViewModel Product { get; set; } = null!;
    public bool ShowProductFeatures { get; set; }
    public AssetViewModel? ProductImage { get; set; }
    public string? CallToAction { get; set; }
    public bool OpenInNewTab { get; set; }
    public bool ShowAdvanced { get; set; }
    public string? ColorScheme { get; set; }
    public string? CornerStyle { get; set; }
    public string? ImagePosition { get; set; }
    public string? ContentAlignmentClass { get; set; }
    public string? CallToActionStyleClasses { get; set; }


    // can be extracted into a parent class and have ProductWidgetViewModel inherit from it
    // see for example Product comparator or Hero banner widget view model
    public bool IsMisconfigured => Product == null;
}
