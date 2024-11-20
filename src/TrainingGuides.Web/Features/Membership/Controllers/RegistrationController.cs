using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using CMS.Core;
using CMS.EmailEngine;

using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;

using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.Registration;
using TrainingGuides.Web.Features.Shared.Services;
using TrainingGuides.Web.Features.Shared.Helpers;

namespace TrainingGuides.Web.Features.Membership.Controllers;

public class RegistrationController(
    IMembershipService membershipService,
    IEventLogService log,
    IStringLocalizer<SharedResources> stringLocalizer,
    IEmailService emailService,
    IOptions<SystemEmailOptions> systemEmailOptions,
    IHttpRequestService httpRequestService) : Controller
{

    private readonly IMembershipService membershipService = membershipService;
    private readonly IEventLogService log = log;
    private readonly IStringLocalizer<SharedResources> stringLocalizer = stringLocalizer;
    private readonly IEmailService emailService = emailService;
    private readonly SystemEmailOptions systemEmailOptions = systemEmailOptions.Value;
    private readonly IHttpRequestService httpRequestService = httpRequestService;

    [HttpPost("{" + ApplicationConstants.LANGUAGE_KEY + "}/Registration/Register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrationWidgetViewModel model)
    {
        var result = IdentityResult.Failed();

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, stringLocalizer["Please fill in all required fields."]);
            return PartialView("~/Features/Membership/Widgets/Registration/RegistrationForm.cshtml", model);
        }

        var guidesMember = new GuidesMember
        {
            UserName = model.UserName,
            Email = model.EmailAddress,
            GivenName = model.GivenName,
            FamilyName = model.FamilyName,
            FamilyNameFirst = model.FamilyNameFirst,
            FavoriteCoffee = model.FavoriteCoffee,
            Enabled = false
        };
        try
        {
            result = await membershipService.CreateMember(guidesMember, model.Password);
        }
        catch (Exception ex)
        {
            log.LogException(nameof(RegistrationController), nameof(Register), ex);
            result = IdentityResult.Failed([new() { Code = "Failure", Description = stringLocalizer["Registration failed."] }]);
        }

        if (result.Succeeded)
        {
            await SendVerificationEmail(guidesMember);
            return Content(stringLocalizer["Success! We've sent you an email. Please confirm your membership by clicking on the link contained in the email."]);
        }
        else
        {
            foreach (string error in result.Errors.Select(e => e.Description))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return PartialView("~/Features/Membership/Widgets/Registration/RegistrationForm.cshtml", model);
        }
    }

    private ActionResult ReturnEmailConfirmationView(EmailConfirmationViewModel emailConfirmationViewModel)
    {
        emailConfirmationViewModel.Title = stringLocalizer["Kentico training guides"];
        return View("~/Features/Membership/Widgets/Registration/EmailConfirmation.cshtml", emailConfirmationViewModel);
    }

    // the system counts on an existence of a sign in anf registration pages with the following URLs
    private string GetSignInUrl() => $"{httpRequestService.GetBaseUrlWithLanguage()}/sign-in";
    private string GetRegisterUrl() => $"{httpRequestService.GetBaseUrlWithLanguage()}/register";

    private EmailConfirmationViewModel GetMemberNotFoundViewModel() => new()
    {
        State = EmailConfirmationState.FailureNotYetConfirmed,
        Message = stringLocalizer["Email confirmation failed. This user does not exist."],
        ActionButtonText = stringLocalizer["Register"],
        SignInOrRegisterPageUrl = GetRegisterUrl(),
        HomePageButtonText = stringLocalizer["Go to homepage"],
        HomePageUrl = httpRequestService.GetBaseUrlWithLanguage()
    };


    [HttpGet("/Registration/Confirm")]
    public async Task<ActionResult> Confirm(string memberEmail, string confirmToken)
    {
        string userName;
        if (!(HttpContext.Kentico().PageBuilder().EditMode || HttpContext.Kentico().Preview().Enabled))
        {
            IdentityResult confirmResult;

            var member = await membershipService.FindMemberByEmail(memberEmail);

            if (member is null)
            {
                return ReturnEmailConfirmationView(GetMemberNotFoundViewModel());
            }

            if (member.Enabled)
            {
                return ReturnEmailConfirmationView(new EmailConfirmationViewModel
                {
                    State = EmailConfirmationState.SuccessAlreadyConfirmed,
                    Message = stringLocalizer["Your email is already verified."],
                    ActionButtonText = stringLocalizer["Sign in"],
                    SignInOrRegisterPageUrl = GetSignInUrl(),
                    HomePageButtonText = stringLocalizer["Go to homepage"],
                    HomePageUrl = httpRequestService.GetBaseUrlWithLanguage()
                });
            }

            try
            {
                //Changes Enabled property of the user
                confirmResult = await membershipService.ConfirmEmail(member, confirmToken);
            }
            catch (InvalidOperationException)
            {
                confirmResult = IdentityResult.Failed(new IdentityError() { Description = stringLocalizer["User not found."] });
            }

            if (confirmResult.Succeeded)
            {
                return ReturnEmailConfirmationView(new EmailConfirmationViewModel
                {
                    State = EmailConfirmationState.SuccessConfirmed,
                    Message = stringLocalizer["Success! Email confirmed."],
                    ActionButtonText = stringLocalizer["Sign in"],
                    SignInOrRegisterPageUrl = GetSignInUrl(),
                    HomePageButtonText = stringLocalizer["Go to homepage"],
                    HomePageUrl = httpRequestService.GetBaseUrlWithLanguage()
                });
            }

            userName = member.UserName!;
        }
        else
        {
            userName = "johnDoe";
        }

        return ReturnEmailConfirmationView(new EmailConfirmationViewModel()
        {
            State = EmailConfirmationState.FailureConfirmationFailed,
            Message = stringLocalizer["Email Confirmation failed"],
            ActionButtonText = stringLocalizer["Send Again"],
            Username = userName
        });
    }

    private async Task SendVerificationEmail(GuidesMember member)
    {
        string confirmToken = await membershipService.GenerateEmailConfirmationToken(member);

        string confirmationURL = Url.Action(nameof(Confirm), "Registration",
        new
        {
            memberEmail = member.Email,
            confirmToken
        },
        Request.Scheme) ?? string.Empty;

        await emailService.SendEmail(new EmailMessage()
        {
            From = $"no-reply@{systemEmailOptions.SendingDomain}",
            Recipients = member.Email,
            Subject = $"Confirm your email here",
            Body = $"""
                <p>To confirm your email address, click <a data-confirmation-url href="{confirmationURL}">here</a>.</p>
                <p style="margin-bottom: 1rem;">You can also copy and paste this URL into your browser.</p>
                <p>{confirmationURL}</p>
                """
        });
    }

    [HttpPost("/Registration/ResendVerificationEmail")]
    public async Task<ActionResult> ResendVerificationEmail(EmailConfirmationViewModel model)
    {
        string userName = model.Username;
        var member = await membershipService.FindMemberByName(userName);

        if (member is null)
        {
            return ReturnEmailConfirmationView(GetMemberNotFoundViewModel());
        }

        await SendVerificationEmail(member);

        return ReturnEmailConfirmationView(new EmailConfirmationViewModel()
        {
            State = EmailConfirmationState.ConfirmationResent,
            Message = stringLocalizer["We have sent you confirmation email containing an email verification link. Please confirm your membership by clicking on the link contained in the email."],
        });
    }
}