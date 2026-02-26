using TrainingGuides.Web.Commerce.Products.Models;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductWidget;

public class ProductWidgetViewModel : IWidgetViewModel
{
    public ProductViewModel? Product { get; set; }

    public string ProductPageUrl { get; set; } = string.Empty;

    public bool ShowVariantSelection { get; set; }

    public bool ShowVariantDetails { get; set; }

    public bool ShowCallToAction { get; set; }

    public string CallToActionText { get; set; } = string.Empty;

    public bool IsMisconfigured => Product is null;
}
