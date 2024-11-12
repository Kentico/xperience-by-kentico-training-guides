using Microsoft.AspNetCore.Identity;

namespace TrainingGuides.Web.Features.Membership.Services;
public interface IMembershipService
{
    public Task<GuidesMember?> GetCurrentMember();
    public Task<bool> IsMemberAuthenticated();
    public Task<IdentityResult> CreateMember(GuidesMember member, string password);
    public Task<SignInResult> SignIn(string userNameOrEmail, string password, bool staySignedIn);
}