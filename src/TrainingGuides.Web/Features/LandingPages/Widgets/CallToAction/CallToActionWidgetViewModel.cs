using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;

public class CallToActionWidgetViewModel: WidgetViewModel
{
    public string? Text { get; set; }
    public string? Url { get; set; }
    public bool OpenInNewTab { get; set; }
    public string? Identifier { get; set; }
    public bool IsDownload { get; set; }
    public override bool IsMisconfigured => string.IsNullOrWhiteSpace(Text) || string.IsNullOrWhiteSpace(Url);

}