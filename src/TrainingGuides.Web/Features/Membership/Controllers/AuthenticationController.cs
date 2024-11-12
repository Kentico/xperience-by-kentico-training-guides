using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMS.Core;
using Htmx;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.Authentication;

namespace TrainingGuides.Web.Features.Membership.Controllers;

[Route("[controller]/[action]")]
public class AccountController : Controller
{
    private readonly IMembershipService membershipService;

    public AccountController(IMembershipService membershipService)
    {
        this.membershipService = membershipService;
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

        var signInResult = await membershipService.SignIn(model.UserNameOrEmail, model.Password, model.StaySignedIn);

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
}