using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;

public class CallToActionWidgetViewModel : WidgetViewModel
{
    public string Text { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool OpenInNewTab { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public bool IsDownload { get; set; }
    public override bool IsMisconfigured => string.IsNullOrWhiteSpace(Text) || string.IsNullOrWhiteSpace(Url);
}