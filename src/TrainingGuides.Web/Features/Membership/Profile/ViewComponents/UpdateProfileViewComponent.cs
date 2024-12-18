using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Services;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.Extensions.Localization;

namespace TrainingGuides.Web.Features.Membership.Profile;

public class UpdateProfileViewComponent : ViewComponent
{
    private readonly IMembershipService membershipService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IUpdateProfileService updateProfileService;
    private readonly IStringLocalizer<SharedResources> stringLocalizer;


    public UpdateProfileViewComponent(IMembershipService membershipService,
        IHttpContextAccessor httpContextAccessor,
        IUpdateProfileService updateProfileService,
        IStringLocalizer<SharedResources> stringLocalizer)
    {
        this.membershipService = membershipService;
        this.httpContextAccessor = httpContextAccessor;
        this.updateProfileService = updateProfileService;
        this.stringLocalizer = stringLocalizer;
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

        model.Title = stringLocalizer["Update profile"];

        return View("~/Features/Membership/Profile/ViewComponents/UpdateProfile.cshtml", model);
    }

}
