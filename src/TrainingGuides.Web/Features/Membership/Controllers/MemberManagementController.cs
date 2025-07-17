using System.Web;
using CMS.EmailEngine;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using TrainingGuides.Web.Features.Membership.Profile;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.ResetPassword;
using TrainingGuides.Web.Features.Shared.Helpers;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Membership.Controllers;

public class MemberManagementController : Controller
{
    private readonly IMembershipService membershipService;
    private readonly IEmailService emailService;
    private readonly IStringLocalizer<SharedResources> stringLocalizer;
    private readonly IHttpRequestService httpRequestService;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    private readonly SystemEmailOptions systemEmailOptions;

    private const string INVALID_PASSWORD_RESET_REQUEST = "Your password reset request is expired or invalid.";
    private const string UPDATE_PROFILE_FORM_VIEW_PATH = "~/Features/Membership/Profile/ViewComponents/UpdateProfileForm.cshtml";
    private const string RESET_PASSWORD_FORM_VIEW_PATH = "~/Features/Membership/Widgets/ResetPassword/ResetPasswordForm.cshtml";
    private const string RESET_PASSWORD_REQUEST_FORM_VIEW_PATH = "~/Features/Membership/Widgets/ResetPassword/ResetPasswordRequestForm.cshtml";
    private const string RESET_PASSWORD_SUCCESS_VIEW_PATH = "~/Features/Membership/Widgets/ResetPassword/ResetPasswordSuccess.cshtml";

    public MemberManagementController(IMembershipService membershipService,
        IEmailService emailService,
        IStringLocalizer<SharedResources> stringLocalizer,
        IHttpRequestService httpRequestService,
        IPreferredLanguageRetriever preferredLanguageRetriever,
        IOptions<SystemEmailOptions> systemEmailOptions)
    {
        this.membershipService = membershipService;
        this.emailService = emailService;
        this.stringLocalizer = stringLocalizer;
        this.httpRequestService = httpRequestService;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
        this.systemEmailOptions = systemEmailOptions.Value;
    }

    /// <summary>
    /// Updates a user profile.
    /// </summary>
    /// <param name="model">View model with profile fields to update.</param>
    /// <returns></returns>
    [HttpPost($"{{{ApplicationConstants.LANGUAGE_KEY}}}{ApplicationConstants.UPDATE_PROFILE_ACTION_PATH}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView(UPDATE_PROFILE_FORM_VIEW_PATH, model);
        }

        //Get the current member instead of pulling from the model, so that members cannot attempt to change each others information.
        var guidesMember = await membershipService.GetCurrentMember();

        if (guidesMember is not null)
        {
            var result = await membershipService.UpdateMemberProfile(guidesMember, model);

            if (result.Succeeded)
            {
                var newModel = GetNewUpdateProfileViewModel(model,
                    guidesMember,
                    stringLocalizer["Profile updated successfully."]);

                return PartialView(UPDATE_PROFILE_FORM_VIEW_PATH, newModel);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        return PartialView(UPDATE_PROFILE_FORM_VIEW_PATH, model);
    }

    /// <summary>
    /// Generates and sends password reset email.
    /// </summary>
    /// <param name="model">ResetPasswordViewModel containing information about the member </param>
    /// <returns></returns>
    [HttpPost($"{{{ApplicationConstants.LANGUAGE_KEY}}}{ApplicationConstants.REQUEST_RESET_PASSWORD_ACTION_PATH}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestPasswordReset(ResetPasswordWidgetViewModel model)
    {
        // Check if the model meets the requirements specified in its data annotations
        if (!ModelState.IsValid)
        {
            return PartialView(RESET_PASSWORD_REQUEST_FORM_VIEW_PATH, model);
        }

        var guidesMember = await membershipService.FindMemberByEmail(model.EmailAddress);

        if (guidesMember is not null && guidesMember.Enabled)
        {
            string token = await membershipService.GeneratePasswordResetToken(guidesMember);

            string encodedToken = HttpUtility.UrlEncode(token);

            string encodedEmail = HttpUtility.UrlEncode(guidesMember.Email) ?? string.Empty;

            string resetUrl = httpRequestService.GetAbsoluteUrlForPath(
                httpRequestService.CombineUrlPaths(ApplicationConstants.PASSWORD_RESET_ACTION_PATH, encodedEmail, encodedToken),
                true);

            await emailService
                .SendEmail(new EmailMessage()
                {
                    From = $"no-reply@{systemEmailOptions.SendingDomain}",
                    Recipients = guidesMember.Email,
                    Subject = stringLocalizer["Password reset request"],
                    Body = $"{stringLocalizer["To reset your account's password, click"]} <a href=\"{resetUrl}\">{stringLocalizer["here"]}</a>.<br/><br/>"
                        + $"{stringLocalizer["You can also copy-paste the following URL into your browser:"]}<br/><br/>"
                        + $"{resetUrl}<br/><br/>"
                        + $"<strong>{stringLocalizer["If you did not request a password reset, please ignore this email, and do not click the link or paste the URL."]}</strong><br/><br/>"
                });
        }
        return Content($"<span>{stringLocalizer["Success!"]}</span><br/><span>{stringLocalizer["If there is an account with this email address, we will send a link right away."]}</span>");
    }

    /// <summary>
    /// Displays the password reset form.
    /// </summary>
    /// <param name="email">Email address of the member to reset</param>
    /// <param name="token">Token generated for the provided member</param>
    /// <returns></returns>
    [HttpGet($"{{{ApplicationConstants.LANGUAGE_KEY}}}{ApplicationConstants.PASSWORD_RESET_ACTION_PATH}/{{email}}/{{token}}")]
    public async Task<IActionResult> ResetPassword(string email, string token)
    {
        string error = string.Empty;
        string invalid = stringLocalizer[INVALID_PASSWORD_RESET_REQUEST];

        if (string.IsNullOrEmpty(token))
            error = invalid;

        string decodedToken = Uri.UnescapeDataString(token);
        string decodedEmail = Uri.UnescapeDataString(email);

        var guidesMember = await membershipService.FindMemberByEmail(decodedEmail);

        bool memberExists = guidesMember is not null && !string.IsNullOrWhiteSpace(guidesMember.Email);
        bool validToken = false;

        if (memberExists)
        {
            try
            {
                validToken = await membershipService.VerifyPasswordResetToken(guidesMember!, decodedToken);
            }
            catch
            {
                error = invalid;
            }
        }
        else
        {
            error = invalid;
        }

        if (!validToken)
            error = invalid;

        if (!string.IsNullOrEmpty(error))
            ModelState.AddModelError(string.Empty, error);

        // If the password request is valid, displays the password reset form
        var model = new ResetPasswordViewModel
        {
            Title = stringLocalizer["Reset password"],
            ActionUrl = httpRequestService.GetAbsoluteUrlForPath(ApplicationConstants.PASSWORD_RESET_ACTION_PATH, true),
            Email = guidesMember?.Email ?? string.Empty,
            Token = decodedToken,
            DisplayForm = memberExists && validToken,
            SubmitButtonText = stringLocalizer["Submit"]
        };

        return View("~/Features/Membership/Widgets/ResetPassword/ResetPassword.cshtml", model);

    }

    /// <summary>
    /// Resets the password of the member.
    /// </summary>
    /// <param name="model">The view model with information about whose password to reset and how</param>
    /// <returns>Model with errors or success message</returns>
    [HttpPost($"{{{ApplicationConstants.LANGUAGE_KEY}}}{ApplicationConstants.PASSWORD_RESET_ACTION_PATH}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView(RESET_PASSWORD_FORM_VIEW_PATH, model);
        }

        string decodedToken = Uri.UnescapeDataString(model.Token);
        string decodedEmail = Uri.UnescapeDataString(model.Email);

        var guidesMember = await membershipService.FindMemberByEmail(decodedEmail);

        if (guidesMember is not null && guidesMember.Enabled)
        {
            var result = await membershipService.ResetPassword(guidesMember, decodedToken, model.Password);

            if (result.Succeeded)
            {
                var successModel = await GetResetPasswordSuccessViewModel();
                return PartialView(RESET_PASSWORD_SUCCESS_VIEW_PATH, successModel);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        else
        {
            model.DisplayForm = false;
            ModelState.AddModelError(string.Empty, stringLocalizer[INVALID_PASSWORD_RESET_REQUEST]);
        }

        return PartialView(RESET_PASSWORD_FORM_VIEW_PATH, model);
    }

    private async Task<ResetPasswordSuccessViewModel> GetResetPasswordSuccessViewModel()
    {
        string language = preferredLanguageRetriever.Get();

        return new ResetPasswordSuccessViewModel
        {
            SignInUrl = await membershipService.GetSignInUrl(language, true),
            BaseUrl = httpRequestService.GetBaseUrlWithLanguage(true),
            SuccessText = stringLocalizer["Success!"],
            SignInLinkText = stringLocalizer["Sign in"],
            HomeLinkText = stringLocalizer["Return to the home page"]
        };
    }

    private UpdateProfileViewModel GetNewUpdateProfileViewModel(UpdateProfileViewModel model, GuidesMember guidesMember, string successMessage) =>
        new()
        {
            Title = model.Title,
            EmailAddress = guidesMember.Email ?? string.Empty,
            UserName = guidesMember.UserName ?? string.Empty,
            Created = guidesMember.Created,
            FullName = guidesMember.FullName,
            GivenName = guidesMember.GivenName,
            FamilyName = guidesMember.FamilyName,
            FamilyNameFirst = guidesMember.FamilyNameFirst,
            FavoriteCoffee = guidesMember.FavoriteCoffee,
            SubmitButtonText = model.SubmitButtonText,
            SuccessMessage = successMessage,
        };
}