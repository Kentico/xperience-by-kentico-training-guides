using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;

public class LinkOrSignOutWidgetViewModel : WidgetViewModel
{
    public override bool IsMisconfigured =>
        string.IsNullOrWhiteSpace(ButtonText)
        || string.IsNullOrWhiteSpace(Url);
    public string Text { get; set; } = string.Empty;
    public string ButtonText { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; }
}