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

    public async Task<GuidesMember?> GetCurrentMember()
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
        var member = await GetCurrentMember();
        return member is not null;
    }

    public async Task<IdentityResult> CreateMember(GuidesMember member, string password) => await userManager.CreateAsync(member, password);

    public async Task<GuidesMember?> GetMemberByUserNameOrEmail(string userNameOrEmail) =>
        await userManager.FindByNameAsync(userNameOrEmail) ?? await userManager.FindByEmailAsync(userNameOrEmail);
}