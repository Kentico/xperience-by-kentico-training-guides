using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.LandingPages.Widgets.HeroBanner;

public class HeroBannerWidgetViewModel : WidgetViewModel
{
    public string? Header { get; set; }
    public HtmlString? Subheader { get; set; }
    public List<BenefitViewModel>? Benefits { get; set; }
    public LinkViewModel? Link { get; set; }
    public AssetViewModel? Media { get; set; }
    public LinkViewModel? BannerLink { get; set; }
    public bool ShowBenefits { get; set; }
    public string? AbsoluteUrl { get; set; }
    public string? PageUrl { get; set; }
    public string? CTALink { get; set; }
    public string? LinkTitle { get; set; }
    public bool DisplayCTA { get; set; }
    public bool OpenInNewTab { get; set; }
    public string? CTAText { get; set; }
    public bool ShowImage { get; set; } = true;
    public bool FullWidth { get; set; } = false;
    public string? TextColor { get; set; }
    public string? ThemeClass { get; set; }
    public HtmlString? StyleAttribute { get; set; }
    public override bool IsMisconfigured => string.IsNullOrEmpty(Header);
}
