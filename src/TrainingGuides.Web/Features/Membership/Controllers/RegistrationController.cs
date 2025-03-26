using CMS.Core;
using CMS.EmailEngine;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.Registration;
using TrainingGuides.Web.Features.Shared.Helpers;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Membership.Controllers;

public class RegistrationController : Controller
{
    private readonly IMembershipService membershipService;
    private readonly IEventLogService log;
    private readonly IStringLocalizer<SharedResources> stringLocalizer;
    private readonly IEmailService emailService;
    private readonly SystemEmailOptions systemEmailOptions;
    private readonly IHttpRequestService httpRequestService;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;

    private const string REGISTRATION_FORM_VIEW_PATH = "~/Features/Membership/Widgets/Registration/RegistrationForm.cshtml";

    public RegistrationController(
    IMembershipService membershipService,
    IEventLogService log,
    IStringLocalizer<SharedResources> stringLocalizer,
    IEmailService emailService,
    IOptions<SystemEmailOptions> systemEmailOptions,
    IHttpRequestService httpRequestService,
    IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.membershipService = membershipService;
        this.log = log;
        this.stringLocalizer = stringLocalizer;
        this.emailService = emailService;
        this.systemEmailOptions = systemEmailOptions.Value;
        this.httpRequestService = httpRequestService;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }

    [HttpPost($"{{{ApplicationConstants.LANGUAGE_KEY}}}{ApplicationConstants.REGISTER_ACTION_PATH}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrationWidgetViewModel model)
    {
        var result = IdentityResult.Failed();

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, stringLocalizer["Please fill in all required fields."]);
            return PartialView(REGISTRATION_FORM_VIEW_PATH, model);
        }

        // NOTE: This example does not include consent, but in a real-world scenario, you may need to get a member's consent before saving this data.
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

            return PartialView(REGISTRATION_FORM_VIEW_PATH, model);
        }
    }

    private ActionResult ReturnEmailConfirmationView(EmailConfirmationViewModel emailConfirmationViewModel)
    {
        emailConfirmationViewModel.Title = stringLocalizer["Kentico training guides"];
        return View("~/Features/Membership/Widgets/Registration/EmailConfirmation.cshtml", emailConfirmationViewModel);
    }

    private async Task<EmailConfirmationViewModel> GetMemberNotFoundViewModel() => new()
    {
        State = EmailConfirmationState.FailureNotYetConfirmed,
        Message = stringLocalizer["Email confirmation failed."],
        ActionButtonText = stringLocalizer["Register"],
        SignInOrRegisterPageUrl = await membershipService.GetRegisterUrl(preferredLanguageRetriever.Get(), true),
        HomePageButtonText = stringLocalizer["Return to the home page"],
        HomePageUrl = httpRequestService.GetBaseUrlWithLanguage(true)
    };

    [HttpGet($"{ApplicationConstants.CONFIRM_REGISTRATION_ACTION_PATH}/{{{ApplicationConstants.LANGUAGE_KEY}}}")]
    public async Task<ActionResult> Confirm(string memberEmail, string confirmToken)
    {
        string userName;
        if (!(HttpContext.Kentico().PageBuilder().GetMode() == PageBuilderMode.Edit
            || HttpContext.Kentico().Preview().Enabled))
        {
            IdentityResult confirmResult;

            var member = await membershipService.FindMemberByEmail(memberEmail);

            if (member is null)
            {
                return ReturnEmailConfirmationView(await GetMemberNotFoundViewModel());
            }

            if (member.Enabled)
            {
                return ReturnEmailConfirmationView(new EmailConfirmationViewModel
                {
                    State = EmailConfirmationState.SuccessAlreadyConfirmed,
                    Message = stringLocalizer["Your email is already verified."],
                    ActionButtonText = stringLocalizer["Sign in"],
                    SignInOrRegisterPageUrl = await membershipService.GetSignInUrl(preferredLanguageRetriever.Get(), true),
                    HomePageButtonText = stringLocalizer["Return to the home page"],
                    HomePageUrl = httpRequestService.GetBaseUrlWithLanguage(true)
                });
            }

            try
            {
                //Changes Enabled property of the user
                confirmResult = await membershipService.ConfirmEmail(member, confirmToken);
            }
            catch
            {
                confirmResult = IdentityResult.Failed(new IdentityError() { Description = stringLocalizer["Email Confirmation failed"] });
            }

            if (confirmResult.Succeeded)
            {
                return ReturnEmailConfirmationView(new EmailConfirmationViewModel
                {
                    State = EmailConfirmationState.SuccessConfirmed,
                    Message = stringLocalizer["Success! Email confirmed."],
                    ActionButtonText = stringLocalizer["Sign in"],
                    SignInOrRegisterPageUrl = await membershipService.GetSignInUrl(preferredLanguageRetriever.Get(), true),
                    HomePageButtonText = stringLocalizer["Return to the home page"],
                    HomePageUrl = httpRequestService.GetBaseUrlWithLanguage(true)
                });
            }

            userName = member.UserName!;
        }
        else
        {
            userName = membershipService.DummyMember.UserName!;
        }

        return ReturnEmailConfirmationView(new EmailConfirmationViewModel()
        {
            State = EmailConfirmationState.FailureConfirmationFailed,
            Message = stringLocalizer["Email Confirmation failed"],
            ActionButtonText = stringLocalizer["Send again"],
            Username = userName
        });
    }

    private async Task SendVerificationEmail(GuidesMember member)
    {
        if (member is null || string.IsNullOrWhiteSpace(member.Email) || member.Enabled)
        {
            return;
        }

        string confirmToken = await membershipService.GenerateEmailConfirmationToken(member);
        string memberEmail = member.Email;

        var routeValues = new RouteValueDictionary
        {
            { ApplicationConstants.LANGUAGE_KEY, preferredLanguageRetriever.Get() },
            { nameof(memberEmail), memberEmail },
            { nameof(confirmToken), confirmToken }
        };

        string confirmationURL = Url.Action(
            nameof(Confirm),
            "Registration",
            routeValues,
            Request.Scheme) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(confirmationURL))
        {
            return;
        }

        await emailService.SendEmail(new EmailMessage()
        {
            From = $"no-reply@{systemEmailOptions.SendingDomain}",
            Recipients = member.Email,
            Subject = $"{stringLocalizer["Confirm your email here"]}",
            Body = $"""
                <p>{stringLocalizer["To confirm your email address, click"]} <a data-confirmation-url href="{confirmationURL}">{stringLocalizer["here"]}</a>.</p>
                <p style="margin-bottom: 1rem;">{stringLocalizer["You can also copy-paste the following URL into your browser:"]}</p>
                <p>{confirmationURL}</p>
                """
        });
    }

    [HttpPost($"{{{ApplicationConstants.LANGUAGE_KEY}}}{ApplicationConstants.RESEND_VERIFICATION_EMAIL}")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ResendVerificationEmail(EmailConfirmationViewModel model)
    {
        string userName = model.Username;
        var member = await membershipService.FindMemberByName(userName);

        if (member is null)
        {
            return ReturnEmailConfirmationView(await GetMemberNotFoundViewModel());
        }

        await SendVerificationEmail(member);

        return ReturnEmailConfirmationView(new EmailConfirmationViewModel()
        {
            State = EmailConfirmationState.ConfirmationResent,
            Message = stringLocalizer["We have sent you confirmation email containing an email verification link. Please confirm your membership by clicking on the link contained in the email."],
        });
    }
}