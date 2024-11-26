using Microsoft.AspNetCore.Identity;

namespace TrainingGuides.Web.Features.Membership.Services;

/// <summary>
/// Interface for membership services.
/// </summary>
public interface IMembershipService
{
    /// <summary>
    /// Gets the current member.
    /// </summary>
    /// <returns>The current <see cref="GuidesMember"/> if found; otherwise, null.</returns>
    Task<GuidesMember?> GetCurrentMember();

    /// <summary>
    /// Checks if the member is authenticated.
    /// </summary>
    /// <returns>True if the member is authenticated; otherwise, false.</returns>
    Task<bool> IsMemberAuthenticated();

    /// <summary>
    /// Creates a new member.
    /// </summary>
    /// <param name="guidesMember">The member to create.</param>
    /// <param name="password">The password for the member.</param>
    /// <returns>The result of the creation operation.</returns>
    Task<IdentityResult> CreateMember(GuidesMember guidesMember, string password);

    /// <summary>
    /// Finds a member by email.
    /// </summary>
    /// <param name="email">The email of the member to find.</param>
    /// <returns>The <see cref="GuidesMember"/> if found; otherwise, null.</returns>
    Task<GuidesMember?> FindMemberByEmail(string email);

    /// <summary>
    /// Finds a member by username.
    /// </summary>
    /// <param name="userName">The username of the member to find.</param>
    /// <returns>The <see cref="GuidesMember"/> if found; otherwise, null.</returns>
    Task<GuidesMember?> FindMemberByName(string userName);

    /// <summary>
    /// Confirms the email of a member.
    /// </summary>
    /// <param name="member">The member whose email is to be confirmed.</param>
    /// <param name="confirmToken">The confirmation token.</param>
    /// <returns>The result of the confirmation operation.</returns>
    Task<IdentityResult> ConfirmEmail(GuidesMember member, string confirmToken);

    /// <summary>
    /// Signs in a member.
    /// </summary>
    /// <param name="userNameOrEmail">The username or email of the member.</param>
    /// <param name="password">The password of the member.</param>
    /// <param name="staySignedIn">Whether to keep the member signed in.</param>
    /// <returns>The result of the sign-in operation.</returns>
    Task<SignInResult> SignIn(string userNameOrEmail, string password, bool staySignedIn);

    /// <summary>
    /// Signs out the current member.
    /// </summary>
    Task SignOut();

    /// <summary>
    /// Generates an email confirmation token for a member.
    /// </summary>
    /// <param name="member">The member for whom to generate the token.</param>
    /// <returns>The email confirmation token.</returns>
    Task<string> GenerateEmailConfirmationToken(GuidesMember member);

    /// <summary>
    /// Generates a password reset token for a member.
    /// </summary>
    /// <param name="member">The member for whom to generate the token.</param>
    /// <returns>The password reset token.</returns>
    Task<string> GeneratePasswordResetToken(GuidesMember member);

    /// <summary>
    /// Verifies a password reset token for a member.
    /// </summary>
    /// <param name="member">The member whose email was used for the request.</param>
    /// <param name="token">The token used for the request.</param>
    /// <returns>True if the token is valid.</returns>
    Task<bool> VerifyPasswordResetToken(GuidesMember member, string token);

    /// <summary>
    /// Resets the password of a member.
    /// </summary>
    /// <param name="member">The member whose password will be updated.</param>
    /// <param name="token">The token used for the request.</param>
    /// <param name="newPassword">The new password.</param>
    /// <returns>The task that represents the async result of the operation, containing an IdentityResult</returns>
    Task<IdentityResult> ResetPassword(GuidesMember member, string token, string newPassword);

    /// <summary>
    /// Gets the URL of the expected sign in page in the provided language.
    /// </summary>
    /// <param name="language">The required language to retrieve.</param>
    /// <param name="absoluteURl">Whether to return an absolute URL.</param>
    /// <returns>The relative path of the sign in page.</returns>
    Task<string> GetSignInUrl(string language, bool absoluteURl = false);
}