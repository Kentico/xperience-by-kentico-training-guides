using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.Authentication;
using TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;

namespace TrainingGuides.Web.Features.Membership.Controllers;



public class AuthenticationController : Controller
{
    private readonly IMembershipService membershipService;
    private const string SIGN_IN_FAILED = "Your sign-in attempt was not successful. Please try again.";

    public AuthenticationController(IMembershipService membershipService)
    {
        this.membershipService = membershipService;
    }

    private IActionResult RenderError(SignInWidgetViewModel model)
    {
        ModelState.AddModelError(string.Empty, SIGN_IN_FAILED);
        return PartialView("~/Features/Membership/Widgets/SignIn/SignInForm.cshtml", model);
    }

    [HttpPost("/Authentication/Authenticate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Authenticate(SignInWidgetViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RenderError(model);
        }

        var signInResult = await membershipService.SignIn(model.UserNameOrEmail, model.Password, model.StaySignedIn);

        if (signInResult.Succeeded)
        {
            string redirectUrl = $"{model.BaseUrl}/home";
            return Redirect(redirectUrl);
        }

        return RenderError(model);
    }

    [Authorize]
    [HttpPost("/Authentication/Logout")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Logout(SignOutFormModel model)
    {
        await membershipService.SignOut();
        return Redirect(model.RedirectUrl);
    }
}