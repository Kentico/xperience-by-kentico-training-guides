using CMS.ContactManagement;
using CMS.Core;
using CMS.Websites.Routing;
using Microsoft.AspNetCore.Identity;
using TrainingGuides.Web.Features.Membership.Profile;
using TrainingGuides.Web.Features.Shared.Helpers;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Membership.Services;
public class MembershipService : IMembershipService
{
    public GuidesMember DummyMember => new()
    {
        UserName = "JohnDoe",
        Email = "JohnDoe@localhost.local",
        GivenName = "John",
        FamilyName = "Doe",
        FamilyNameFirst = false,
        FavoriteCoffee = "Latte",
        Enabled = true,
        Created = DateTime.Now,
        Id = 0
    };

    private readonly UserManager<GuidesMember> userManager;
    private readonly SignInManager<GuidesMember> signInManager;
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IEventLogService eventLogService;
    private readonly IMemberContactService memberContactService;
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    private readonly IWebsiteChannelContext websiteChannelContext;
    private readonly IHttpRequestService httpRequestService;


    public MembershipService(
        UserManager<GuidesMember> userManager,
        SignInManager<GuidesMember> signInManager,
        IHttpContextAccessor contextAccessor,
        IEventLogService eventLogService,
        IMemberContactService memberContactService,
        IWebPageUrlRetriever webPageUrlRetriever,
        IWebsiteChannelContext websiteChannelContext,
        IHttpRequestService httpRequestService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.contextAccessor = contextAccessor;
        this.eventLogService = eventLogService;
        this.memberContactService = memberContactService;
        this.webPageUrlRetriever = webPageUrlRetriever;
        this.websiteChannelContext = websiteChannelContext;
        this.httpRequestService = httpRequestService;
    }

    /// <inheritdoc />
    public async Task<GuidesMember?> GetCurrentMember()
    {
        var context = contextAccessor.HttpContext;
        if (context is null)
        {
            return null;
        }

        return await userManager.GetUserAsync(context.User);
    }

    /// <inheritdoc />
    public async Task<bool> IsMemberAuthenticated()
    {
        var member = await GetCurrentMember();
        return member is not null;
    }

    /// <inheritdoc />
    public async Task<IdentityResult> CreateMember(GuidesMember guidesMember, string password) =>
        await userManager.CreateAsync(guidesMember, password);

    private async Task<GuidesMember?> FindMemberByUserNameOrEmail(string userNameOrEmail) =>
        await userManager.FindByNameAsync(userNameOrEmail) ?? await userManager.FindByEmailAsync(userNameOrEmail);

    /// <inheritdoc />
    public async Task<GuidesMember?> FindMemberByName(string userName) =>
        await userManager.FindByNameAsync(userName);

    /// <inheritdoc />
    public async Task<GuidesMember?> FindMemberByEmail(string email) =>
        await userManager.FindByEmailAsync(email);

    /// <inheritdoc />
    public async Task<IdentityResult> ConfirmEmail(GuidesMember member, string confirmToken) =>
        await userManager.ConfirmEmailAsync(member, confirmToken);

    /// <inheritdoc />
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

                memberContactService.MergeContactByEmail(contact);

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

    /// <inheritdoc />
    public async Task SignOut()
    {
        await signInManager.SignOutAsync();

        memberContactService.RemoveContactCookies();
    }

    /// <inheritdoc />
    public async Task<string> GenerateEmailConfirmationToken(GuidesMember member) =>
        await userManager.GenerateEmailConfirmationTokenAsync(member);

    /// <inheritdoc />
    public async Task<string> GeneratePasswordResetToken(GuidesMember member) =>
        await userManager.GeneratePasswordResetTokenAsync(member);

    /// <inheritdoc />
    public async Task<bool> VerifyPasswordResetToken(GuidesMember member, string token) =>
        await userManager.VerifyUserTokenAsync(user: member,
            tokenProvider: userManager.Options.Tokens.PasswordResetTokenProvider,
            purpose: UserManager<GuidesMember>.ResetPasswordTokenPurpose,
            token: token);

    /// <inheritdoc />
    public async Task<IdentityResult> ResetPassword(GuidesMember member, string token, string password) =>
        await userManager.ResetPasswordAsync(member, token, password);

    private async Task<string> GetPageUrl(string expectedPagePath, string language, bool absoluteURL = false)
    {
        var signInUrl = await webPageUrlRetriever.Retrieve(
            webPageTreePath: expectedPagePath,
            websiteChannelName: websiteChannelContext.WebsiteChannelName,
            languageName: language
        );

        return absoluteURL ?
            signInUrl.RelativePath.Replace("~", httpRequestService.GetBaseUrl())
            : signInUrl.RelativePath;
    }

    /// <inheritdoc />
    public async Task<string> GetSignInUrl(string language, bool absoluteURL = false)
        => await GetPageUrl(ApplicationConstants.EXPECTED_SIGN_IN_PATH, language, absoluteURL);

    /// <inheritdoc />
    public async Task<string> GetRegisterUrl(string language, bool absoluteURL = false)
        => await GetPageUrl(ApplicationConstants.EXPECTED_REGISTER_PATH, language, absoluteURL);


    /// <inheritdoc />
    public async Task<IdentityResult> UpdateMemberProfile(GuidesMember guidesMember, UpdateProfileViewModel updateProfileViewModel)
    {
        guidesMember.GivenName = updateProfileViewModel.GivenName;
        guidesMember.FamilyName = updateProfileViewModel.FamilyName;
        guidesMember.FamilyNameFirst = updateProfileViewModel.FamilyNameFirst;
        guidesMember.FavoriteCoffee = updateProfileViewModel.FavoriteCoffee;

        return await userManager.UpdateAsync(guidesMember);
    }
}