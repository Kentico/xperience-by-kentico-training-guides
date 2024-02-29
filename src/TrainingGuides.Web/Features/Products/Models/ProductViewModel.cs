using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Products.Models;

public class ProductViewModel : PageViewModel
{
    public HtmlString Name { get; set; } = null!;
    public string Header { get; set; } = null!;
    public HtmlString ShortDescription { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<AssetViewModel?> Media { get; set; } = [];
    public LinkViewModel? Link { get; set; }
    public string CallToAction { get; set; } = null!;
    public decimal AnnualRate { get; set; }
    public decimal InterestRate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public int LoanDuration { get; set; }
    public string? LoanType { get; set; }
    public bool RenegotiationableLoan { get; set; }
    public string RequiredCollateral { get; set; } = null!;
    public List<ProductFeaturesViewModel> Features { get; set; } = [];
}
