using Microsoft.AspNetCore.Mvc;

namespace TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;

public class SignOutFormModel
{
    [HiddenInput]
    public string RedirectUrl { get; set; } = string.Empty;
    public string ButtonText { get; set; } = string.Empty;
}