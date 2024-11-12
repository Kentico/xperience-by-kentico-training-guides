using CMS.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TrainingGuides.Admin;
using TrainingGuides.Web.Features.Membership;

public class RegistrationController(UserManager<GuidesMember> userManager, IEventLogService log, IStringLocalizer<SharedResources> localizer) : Controller
{

    [HttpPost("/Registration/Register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrationWidgetViewModel model)
    {
        var result = IdentityResult.Failed();

        if (ModelState.IsValid)
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

            try
            {
                result = await userManager.CreateAsync(guidesMember, model.Password);
            }
            catch (Exception ex)
            {
                log.LogException(nameof(RegistrationController), nameof(Register), ex);
                result = IdentityResult.Failed([new() { Code = "Failure", Description = localizer["Registration failed."] }]);
            }
        }

        if (result.Succeeded)
        {
            return Content(localizer["Success!"]);
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
}