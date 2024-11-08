using CMS.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Membership;

public class RegistrationController(UserManager<GuidesMember> userManager, IEventLogService log, IStringLocalizer localizer) : Controller
{

    [HttpPost("/Registration/Register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var guidesMember = new GuidesMember
        {
            UserName = model.UserName,
            Email = model.EmailAddress,
            GivenName = model.GivenName,
            FamilyName = model.FamilyName,
            FamilyNameFirst = model.FamilyNameFirst,
            FavoriteCoffee = model.FavoriteCoffee
        };

        var result = IdentityResult.Failed();
        try
        {
            result = await userManager.CreateAsync(guidesMember, model.Password);
        }
        catch (Exception ex)
        {
            log.LogException(nameof(RegistrationController), nameof(Register), ex);
            result = IdentityResult.Failed([new() { Code = "Failure", Description = "Your registration was not successful." }]);
        }

        if (result.Succeeded)
        {
            return Ok(localizer["Success"]);
        }
        else
        {
            foreach (string error in result.Errors.Select(e => e.Description))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return PartialView("~/Features/Registration/_RegisterForm.cshtml", model);
        }
    }
}