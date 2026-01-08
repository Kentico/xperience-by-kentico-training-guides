using TrainingGuides.Web.Features.FinancialServices.Models;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.FinancialServices.Widgets.Service;

public class ServiceWidgetViewModel : IWidgetViewModel
{
    public ServicePageViewModel? Service { get; set; }
    public bool ShowServiceFeatures { get; set; }
    public AssetViewModel ServiceImage { get; set; } = new();
    public bool ShowAdvanced { get; set; }
    public string ParentElementCssClasses { get; set; } = string.Empty;
    public string MainContentElementCssClasses { get; set; } = string.Empty;
    public string ImageElementCssClasses { get; set; } = string.Empty;
    public string ColorScheme { get; set; } = string.Empty;
    public string CornerStyle { get; set; } = string.Empty;
    public string CallToActionCssClasses { get; set; } = string.Empty;
    public bool IsImagePositionSide { get; set; } = false;

    public bool IsMisconfigured => Service is null;
}
