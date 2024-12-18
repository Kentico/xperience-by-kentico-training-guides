using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.LandingPages.Widgets.HeroBanner;

public class HeroBannerWidgetViewModel : IWidgetViewModel
{
    public string Header { get; set; } = string.Empty;
    public HtmlString Subheader { get; set; } = HtmlString.Empty;
    public List<BenefitViewModel> Benefits { get; set; } = [];
    public LinkViewModel Link { get; set; } = new LinkViewModel();
    public AssetViewModel Media { get; set; } = new AssetViewModel();
    public LinkViewModel BannerLink { get; set; } = new LinkViewModel();
    public bool ShowBenefits { get; set; }
    public string AbsoluteUrl { get; set; } = string.Empty;
    public string PageUrl { get; set; } = string.Empty;
    public string CTALink { get; set; } = string.Empty;
    public string LinkTitle { get; set; } = string.Empty;
    public bool DisplayCTA { get; set; }
    public bool OpenInNewTab { get; set; }
    public string CTAText { get; set; } = string.Empty;
    public bool ShowImage { get; set; } = true;
    public bool FullWidth { get; set; } = false;
    public string TextColor { get; set; } = string.Empty;
    public string ThemeClass { get; set; } = string.Empty;
    public HtmlString StyleAttribute { get; set; } = HtmlString.Empty;
    public bool IsMisconfigured => string.IsNullOrEmpty(Header);
}
