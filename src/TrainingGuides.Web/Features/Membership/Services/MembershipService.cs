using CMS.ContactManagement;
using CMS.Core;
using Microsoft.AspNetCore.Identity;

namespace TrainingGuides.Web.Features.Membership.Services;
public class MembershipService : IMembershipService
{
    private readonly UserManager<GuidesMember> userManager;
    private readonly SignInManager<GuidesMember> signInManager;
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IEventLogService eventLogService;
    private readonly IMemberContactService memberContactService;

    public MembershipService(
        UserManager<GuidesMember> userManager,
        SignInManager<GuidesMember> signInManager,
        IHttpContextAccessor contextAccessor,
        IEventLogService eventLogService,
        IMemberContactService memberContactService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.contextAccessor = contextAccessor;
        this.eventLogService = eventLogService;
        this.memberContactService = memberContactService;
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

    public async Task<IdentityResult> CreateMember(GuidesMember guidesMember, string password) =>
        await userManager.CreateAsync(guidesMember, password);

    private async Task<GuidesMember?> FindMemberByUserNameOrEmail(string userNameOrEmail) =>
        await userManager.FindByNameAsync(userNameOrEmail) ?? await userManager.FindByEmailAsync(userNameOrEmail);

    public async Task<GuidesMember?> FindMemberByName(string userName) =>
        await userManager.FindByNameAsync(userName);

    public async Task<GuidesMember?> FindMemberByEmail(string email) =>
        await userManager.FindByEmailAsync(email);

    public async Task<IdentityResult> ConfirmEmail(GuidesMember member, string confirmToken) =>
        await userManager.ConfirmEmailAsync(member, confirmToken);

    public async Task<SignInResult> SignIn(string userNameOrEmail, string password, bool staySignedIn)
    {
        try
        {
            var member = await FindMemberByUserNameOrEmail(userNameOrEmail);
            if (member is null)
            {
                return SignInResult.Failed;
            }

            var signInResult = await signInManager.PasswordSignInAsync(member.UserName!, password, staySignedIn, false);

            if (signInResult.Succeeded)
            {
                var contact = ContactManagementContext.GetCurrentContact() ?? new ContactInfo();

                contact = memberContactService.TransferMemberFieldsToContact(member, contact);

                memberContactService.UpdateContactIfChanged(contact);

                memberContactService.SetCurrentContactForMember(member);
            }

            return signInResult;
        }
        catch (Exception ex)
        {
            eventLogService.LogException(nameof(MembershipService), nameof(SignIn), ex);
            return SignInResult.Failed;
        }
    }

    public async Task SignOut()
    {
        await signInManager.SignOutAsync();

        memberContactService.RemoveContactCookies();
    }

    public async Task<string> GenerateEmailConfirmationToken(GuidesMember member) =>
        await userManager.GenerateEmailConfirmationTokenAsync(member);
}
