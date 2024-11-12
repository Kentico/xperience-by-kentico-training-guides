using Microsoft.AspNetCore.Identity;

namespace TrainingGuides.Web.Features.Membership.Services;
public interface IMembershipService
{
    public Task<GuidesMember?> GetCurrentMember();
    public Task<bool> IsMemberAuthenticated();
    public Task<IdentityResult> CreateMember(GuidesMember member, string password);
    public Task<GuidesMember?> GetMemberByUserNameOrEmail(string userNameOrEmail);
}