using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.FinancialServices.Models;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.FinancialServices.Widgets.ServiceComparator;

public class ServiceComparatorWidgetViewModel : IWidgetViewModel
{
    public List<ServicePageViewModel> Services { get; set; } = [];
    public List<KeyValuePair<string, HtmlString>> GroupedFeaturesHtmlDictionary { get; set; } = [];
    public string ComparatorHeading { get; set; } = string.Empty;
    public string HeadingType { get; set; } = string.Empty;
    public string HeadingMargin { get; set; } = string.Empty;
    public bool ShowShortDescription { get; set; }
    public bool IsMisconfigured => Services.Count == 0;
}
