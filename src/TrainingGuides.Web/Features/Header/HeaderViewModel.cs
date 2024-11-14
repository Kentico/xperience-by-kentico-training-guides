using TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;

namespace TrainingGuides.Web.Features.Header;

public class HeaderViewModel
{
    public string Heading { get; set; } = string.Empty;
    public LinkOrSignOutWidgetProperties LinkOrSignOutWidgetProperties { get; set; } = new();
}
