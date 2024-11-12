using System.Web;
using CMS.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Widgets.Authentication;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Htmx;
using TrainingGuides.Web.Features.Membership.Services;


namespace TrainingGuides.Web.Features.Membership.Controllers;

[Route("[controller]/[action]")]
public class AccountController : Controller
{
    private readonly IEventLogService eventLogService;
    private readonly IMembershipService membershipService;
    private readonly SignInManager<GuidesMember> signInManager;

    public AccountController(
        IMembershipService membershipService,
        SignInManager<GuidesMember> signInManager,
        IEventLogService eventLogService)
    {
        this.membershipService = membershipService;
        this.signInManager = signInManager;
        this.eventLogService = eventLogService;
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(SignInWidgetViewModel model, string returnUrl)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("~/Features/Widgets/Authentication/SignInForm.cshtml", model);
        }

        var signInResult = SignInResult.Failed;
        try
        {
            var member = await membershipService.GetMemberByUserNameOrEmail(model.UserNameOrEmail);

            signInResult = member is null
                ? SignInResult.Failed
                : await signInManager.PasswordSignInAsync(member.UserName!, model.Password, model.StaySignedIn, false);
        }
        catch (Exception ex)
        {
            eventLogService.LogException(nameof(AuthenticationController), nameof(SignIn), ex);

            signInResult = SignInResult.Failed;
        }

        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Your sign-in attempt was not successful. Please try again.");

            return PartialView("~/Features/Widgets/Authentication/SignInForm.cshtml", model);
        }

        string decodedReturnUrl = HttpUtility.UrlDecode(returnUrl) ?? "";

        string redirectUrl = $"{Request.PathBase}/home";

        Response.Htmx(h => h.Redirect(redirectUrl));

        return Request.IsHtmx()
            ? Ok()
            : Redirect(redirectUrl);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return Redirect($"{Request.PathBase}/profile");
    }
}