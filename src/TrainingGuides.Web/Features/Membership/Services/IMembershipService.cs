using Microsoft.AspNetCore.Identity;

namespace TrainingGuides.Web.Features.Membership.Services;
public interface IMembershipService
{
    public Task<GuidesMember?> GetCurrentMember();
    public Task<bool> IsMemberAuthenticated();
    public Task<IdentityResult> CreateMember(GuidesMember guidesMember, string password);
    public Task<GuidesMember?> FindMemberByEmail(string email);
    public Task<GuidesMember?> FindMemberByName(string userName);
    public Task<IdentityResult> ConfirmEmail(GuidesMember member, string confirmToken);
    public Task<SignInResult> SignIn(string userNameOrEmail, string password, bool staySignedIn);
    public Task SignOut();
    public Task<string> GenerateEmailConfirmationToken(GuidesMember member);
}