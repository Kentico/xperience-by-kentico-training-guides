using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Services;

namespace TrainingGuides.Web.Features.Membership.Profile;

public class UpdateProfileViewComponent(
    IMembershipService membershipService,
    IGuidesRoleService guidesRoleService,
    IHttpContextAccessor httpContextAccessor,
    IUpdateProfileService updateProfileService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;
        GuidesMember currentMember;

        bool useDummyMember = httpContext.Kentico().PageBuilder().GetMode() != PageBuilderMode.Off || httpContext.Kentico().Preview().Enabled;

        currentMember = useDummyMember
            ? membershipService.DummyMember
            : await membershipService.GetCurrentMember() ?? membershipService.DummyMember;

        var roles = useDummyMember
            ? []
            : await guidesRoleService.GetRoles(currentMember);

        var model = updateProfileService.GetViewModel(currentMember, roles);

        return View("~/Features/Membership/Profile/ViewComponents/UpdateProfile.cshtml", model);
    }
}
