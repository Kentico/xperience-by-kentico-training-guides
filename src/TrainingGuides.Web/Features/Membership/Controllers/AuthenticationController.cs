using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.SignIn;
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

        return signInResult.Succeeded
            ? Content("Success!")
            : RenderError(model);
    }

    [Authorize]
    [HttpPost("/Authentication/SignOut")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SignOut(SignOutFormModel model)
    {
        await membershipService.SignOut();
        return Redirect(model.RedirectUrl);
    }
}
