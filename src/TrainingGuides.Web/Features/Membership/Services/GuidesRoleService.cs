using Microsoft.AspNetCore.Identity;

namespace TrainingGuides.Web.Features.Membership.Services;

public class GuidesRoleService(
    UserManager<GuidesMember> userManager,
    RoleManager<GuidesRole> roleManager,
    IHttpContextAccessor httpContextAccessor) : IGuidesRoleService
{
    public async Task<IReadOnlyCollection<GuidesRole>> GetCurrentMemberRoles()
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext?.User is null)
        {
            return [];
        }

        var member = await userManager.GetUserAsync(httpContext.User);

        return member is null
            ? []
            : await GetRoles(member);
    }

    public async Task<IReadOnlyCollection<GuidesRole>> GetRoles(GuidesMember member)
    {
        if (member is null)
        {
            return [];
        }

        var roleNames = await userManager.GetRolesAsync(member);
        var roles = new List<GuidesRole>();

        foreach (string roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);

            if (role is not null)
            {
                roles.Add(role);
            }
        }

        return roles
            .OrderBy(role => GetRolePriority(role.Name))
            .ToArray();
    }

    public async Task<GuidesRole?> GetHighestPriorityRole(GuidesMember member) =>
        (await GetRoles(member)).FirstOrDefault();

    public async Task<bool> IsInRole(GuidesMember member, string roleName) =>
        member is not null && await userManager.IsInRoleAsync(member, roleName);

    /// <summary>
    /// Creates a member role when it does not already exist.
    /// Intended for controlled provisioning or installation flows, not app startup seeding.
    /// </summary>
    public async Task<IdentityResult> CreateRoleIfNotExists(GuidesRole role)
    {
        if (await roleManager.RoleExistsAsync(role.Name ?? string.Empty))
        {
            return IdentityResult.Success;
        }

        return await roleManager.CreateAsync(role);
    }

    private static int GetRolePriority(string? roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return int.MaxValue;
        }

        int index = Array.IndexOf(GuidesRoleNames.DisplayPriority.ToArray(), roleName);

        return index >= 0 ? index : int.MaxValue;
    }
}
