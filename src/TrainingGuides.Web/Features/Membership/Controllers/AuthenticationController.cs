using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.Authentication;
using TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;

namespace TrainingGuides.Web.Features.Membership.Controllers;

public class AuthenticationController : Controller
{
    private readonly IMembershipService membershipService;

    public AuthenticationController(IMembershipService membershipService)
    {
        this.membershipService = membershipService;
    }

    [HttpPost("/Authentication/Authenticate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Authenticate(SignInWidgetViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Your sign-in attempt was not successful. Please try again.");
            return PartialView("~/Features/Membership/Widgets/Authentication/SignInForm.cshtml", model);
        }

        var signInResult = await membershipService.SignIn(model.UserNameOrEmail, model.Password, model.StaySignedIn);

        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Your sign-in attempt was not successful. Please try again.");
            return PartialView("~/Features/Membership/Widgets/Authentication/SignInForm.cshtml", model);
        }

        string redirectUrl = $"{Request.PathBase}/home";
        return Redirect(redirectUrl);
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