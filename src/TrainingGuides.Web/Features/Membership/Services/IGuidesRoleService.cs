using Microsoft.AspNetCore.Identity;

namespace TrainingGuides.Web.Features.Membership.Services;

public interface IGuidesRoleService
{
    Task<IReadOnlyCollection<GuidesRole>> GetCurrentMemberRoles();

    Task<IReadOnlyCollection<GuidesRole>> GetRoles(GuidesMember member);

    Task<GuidesRole?> GetHighestPriorityRole(GuidesMember member);

    Task<bool> IsInRole(GuidesMember member, string roleName);

    Task<IdentityResult> CreateRoleIfNotExists(GuidesRole role);
}
