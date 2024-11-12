using Microsoft.AspNetCore.Identity;

namespace TrainingGuides.Web.Features.Membership.Services;
public class MembershipService : IMembershipService
{
    private readonly UserManager<GuidesMember> userManager;
    private readonly IHttpContextAccessor contextAccessor;
    public MembershipService(
        UserManager<GuidesMember> userManager,
        IHttpContextAccessor contextAccessor)
    {
        this.userManager = userManager;
        this.contextAccessor = contextAccessor;
    }

    public async Task<GuidesMember?> GetMember()
    {
        var context = contextAccessor.HttpContext;
        if (context is null)
        {
            return null;
        }

        return await userManager.GetUserAsync(context.User);
    }
    public async Task<bool> IsMemberAuthenticated()
    {
        var member = await GetMember();
        return member is not null;
    }
}