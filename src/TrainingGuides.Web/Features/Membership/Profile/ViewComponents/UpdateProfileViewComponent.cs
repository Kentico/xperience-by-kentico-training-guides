using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Services;

namespace TrainingGuides.Web.Features.Membership.Profile;

public class UpdateProfileViewComponent : ViewComponent
{
    private readonly IMembershipService membershipService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IUpdateProfileService updateProfileService;

    public UpdateProfileViewComponent(IMembershipService membershipService,
        IHttpContextAccessor httpContextAccessor,
        IUpdateProfileService updateProfileService)
    {
        this.membershipService = membershipService;
        this.httpContextAccessor = httpContextAccessor;
        this.updateProfileService = updateProfileService;

    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;
        GuidesMember currentMember;

        bool useDummyMember = httpContext.Kentico().PageBuilder().GetMode() != PageBuilderMode.Off || httpContext.Kentico().Preview().Enabled;

        currentMember = useDummyMember
            ? membershipService.DummyMember
            : await membershipService.GetCurrentMember() ?? membershipService.DummyMember;

        var model = updateProfileService.GetViewModel(currentMember);

        return View("~/Features/Membership/Profile/ViewComponents/UpdateProfile.cshtml", model);
    }
}
