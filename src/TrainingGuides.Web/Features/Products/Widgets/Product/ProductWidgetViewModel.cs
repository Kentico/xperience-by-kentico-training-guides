using TrainingGuides.Web.Features.Products.Models;

namespace TrainingGuides.Web.Features.Products.Widgets.Product;

public class ProductWidgetViewModel
{
    public ProductViewModel Product { get; set; } = null!;
    public bool ShowProductFeatures { get; set; }
    public bool ShowProductImage { get; set; }
    public bool OpenProductPageOnClick { get; set; }
    public string? CallToAction { get; set; }
    public bool OpenInNewTab { get; set; }
    public bool ShowAdvanced { get; set; }
    public string? CardSize { get; set; }
    public string? ColorScheme { get; set; }
    public string? CornerStyle { get; set; }
    public string? ContentAlignment { get; set; }

    // can be extracted into a parent class and have ProductWidgetViewModel inherit from it
    // see for example Product comparator or Hero banner widget view model
    public bool IsMisconfigured => false;
}
