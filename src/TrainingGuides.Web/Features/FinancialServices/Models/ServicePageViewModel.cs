using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.FinancialServices.Models;

public class ServicePageViewModel : PageViewModel
{
    public HtmlString NameHtml { get; set; } = HtmlString.Empty;
    public HtmlString ShortDescriptionHtml { get; set; } = HtmlString.Empty;
    public HtmlString DescriptionHtml { get; set; } = HtmlString.Empty;
    public List<AssetViewModel> Media { get; set; } = [];
    public decimal Price { get; set; }
    public List<ServiceFeatureViewModel> Features { get; set; } = [];
    public List<BenefitViewModel> Benefits { get; set; } = [];
}
