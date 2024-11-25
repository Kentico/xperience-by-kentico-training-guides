using System.Web;
using CMS.EmailEngine;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
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

    private const string INVALID_PASSWORD_RESET_REQUEST = "Your password reset request is expired or invalid.";

    public MemberManagementController(IMembershipService membershipService,
        IEmailService emailService,
        IStringLocalizer<SharedResources> stringLocalizer,
        IHttpRequestService httpRequestService,
        IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.membershipService = membershipService;
        this.emailService = emailService;
        this.stringLocalizer = stringLocalizer;
        this.httpRequestService = httpRequestService;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }

    /// <summary>
    /// Generates and sends password reset email.
    /// </summary>
    /// <param name="model">ResetPasswordViewModel containing information about the member </param>
    /// <returns></returns>
    [HttpPost($"{{{ApplicationConstants.LANGUAGE_KEY}}}{ApplicationConstants.REQUEST_RESET_PASSWORD_ACTION_PATH}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestResetPassword(ResetPasswordWidgetViewModel model)
    {
        // Check if the model meets the requirements specified in its data annotations
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var guidesMember = await membershipService.FindMemberByEmail(model.EmailAddress);

        if (guidesMember != null)
        {
            string token = await membershipService.GeneratePasswordResetToken(guidesMember);

            string encodedToken = HttpUtility.UrlEncode(token);

            string encodedEmail = HttpUtility.UrlEncode(guidesMember.Email) ?? string.Empty;

            string resetUrl = $"{model.BaseUrlWithLanguage}{ApplicationConstants.PASSWORD_RESET_ACTION_PATH}/{encodedEmail}/{encodedToken}";

            await emailService
                .SendEmail(new EmailMessage()
                {
                    From = "admin@localhost.local",
                    Recipients = guidesMember.Email,
                    Subject = stringLocalizer["Password reset request"],
                    Body = $"{stringLocalizer["To reset your account's password, click"]} <a href=\"{resetUrl}\">{stringLocalizer["here"]}</a>.<br/><br/>"
                        + $"<strong>{stringLocalizer["If you did not request a password reset, please ignore this email, and do not click the link."]}</strong><br/><br/>"
                        + $"{stringLocalizer["You can also copy-paste the following URL into your browser:"]}<br/><br/>"
                        + resetUrl
                });
        }
        // Don't return different results based on whether the email exists or not - this can be used to determine valid emails.
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

        string decodedToken = token.Replace("%2f", "/");
        string decodedEmail = HttpUtility.UrlDecode(email);

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
            BaseUrlWithLanguage = $"{httpRequestService.GetBaseUrl()}/{preferredLanguageRetriever.Get()}",
            Email = guidesMember?.Email ?? string.Empty,
            Token = decodedToken,
            DisplayForm = memberExists && validToken,
            SubmitButtonText = stringLocalizer["Submit"]
        };

        return View("~/Features/Membership/Widgets/ResetPassword/ResetPassword.cshtml", model);

    }

    [HttpPost($"{{{ApplicationConstants.LANGUAGE_KEY}}}{ApplicationConstants.PASSWORD_RESET_ACTION_PATH}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("~/Features/Membership/Widgets/ResetPassword/ResetPasswordForm.cshtml", model);
        }

        string decodedToken = model.Token.Replace("%2f", "/");

        var guidesMember = await membershipService.FindMemberByEmail(model.Email);

        if (guidesMember != null)
        {
            var result = await membershipService.ResetPassword(guidesMember, decodedToken, model.Password);

            if (result.Succeeded)
            {
                return Content(await GetSuccessContent());
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

        return PartialView("~/Features/Membership/Widgets/ResetPassword/ResetPasswordForm.cshtml", model);
    }

    private async Task<string> GetSuccessContent()
    {
        string language = preferredLanguageRetriever.Get();
        string signInUrl = await membershipService.GetSignInUrl(language);
        string baseUrl = httpRequestService.GetBaseUrlWithLanguage();

        string success = stringLocalizer["Success!"];
        string signIn = stringLocalizer["Sign in"];
        string goHome = stringLocalizer["Return to the home page"];

        return $"<div><span>{success}</span></div>"
            + $"<div><span><a href=\"{signInUrl}\">{signIn}</a></span></div>"
            + $"<div><span><a href=\"{baseUrl}\">{goHome}</a></span></div>";
    }
}