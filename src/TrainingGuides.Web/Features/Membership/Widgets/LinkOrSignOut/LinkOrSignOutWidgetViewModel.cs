namespace TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;

public class LinkOrSignOutWidgetViewModel
{
    public string Text { get; set; } = string.Empty;
    public string ButtonText { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; }
}