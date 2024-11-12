using Microsoft.AspNetCore.Identity;

namespace TrainingGuides.Web.Features.Membership.Services;
public interface IMembershipService
{
    public Task<GuidesMember?> GetMember();
    public Task<bool> IsMemberAuthenticated();
    public Task<IdentityResult> CreateMember(GuidesMember member, string password);
}