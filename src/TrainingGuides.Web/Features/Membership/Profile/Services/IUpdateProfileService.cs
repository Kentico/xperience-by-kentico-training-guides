namespace TrainingGuides.Web.Features.Membership.Profile;

public interface IUpdateProfileService
{
    /// <summary>
    /// Get the view model for the update profile view component.
    /// </summary>
    /// <param name="guidesMember">The member to base the view model on.</param>
    /// <param name="roles">The roles of the member to include in the view model.</param>
    /// <returns>An <see cref="UpdateProfileViewModel"/> based on the values of the <see cref="GuidesMember"/>'s properties.</returns>
    UpdateProfileViewModel GetViewModel(GuidesMember guidesMember, IEnumerable<GuidesRole> roles);
}
