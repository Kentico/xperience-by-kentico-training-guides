using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.LandingPages.Widgets.HeroBanner;

public class HeroBannerWidgetViewModel : WidgetViewModel
{
    public string Header { get; set; } = null!;
    public HtmlString Subheader { get; set; } = null!;
    public List<BenefitViewModel> Benefits { get; set; } = null!;
    public LinkViewModel Link { get; set; } = null!;
    public AssetViewModel? Media { get; set; }
    public LinkViewModel BannerLink { get; set; } = null!;
    public bool ShowBenefits { get; set; }
    public string AbsoluteUrl { get; set; } = null!;
    public string PageUrl { get; set; } = null!;
    public string CTALink { get; set; } = null!;
    public string LinkTitle { get; set; } = null!;
    public bool DisplayCTA { get; set; }
    public bool OpenInNewTab { get; set; }
    public string CTAText { get; set; } = null!;
    public bool ShowImage { get; set; } = true;
    public bool FullWidth { get; set; } = false;
    public string TextColor { get; set; } = null!;
    public string ThemeClass => TextColor switch
    {
        "light" => "light",
        "dark" => "",
        _ => ""
    };
    public HtmlString StyleAttribute { get; set; } = null!;
    public override bool IsMisconfigured => string.IsNullOrEmpty(Header);
}
