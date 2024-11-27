using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.SignIn;
using TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;
using TrainingGuides.Web.Features.Shared.Helpers;
using Kentico.Content.Web.Mvc.Routing;
using CMS.DataEngine;
using CMS.ContentEngine;
using Microsoft.Extensions.Localization;

namespace TrainingGuides.Web.Features.Membership.Controllers;



public class AuthenticationController : Controller
{
    private readonly IMembershipService membershipService;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider;
    private readonly IStringLocalizer<SharedResources> stringLocalizer;

    private const string SIGN_IN_FAILED = "Your sign-in attempt was not successful. Please try again.";

    public AuthenticationController(IMembershipService membershipService,
        IPreferredLanguageRetriever preferredLanguageRetriever,
        IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider,
        IStringLocalizer<SharedResources> stringLocalizer)
    {
        this.membershipService = membershipService;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
        this.contentLanguageInfoProvider = contentLanguageInfoProvider;
        this.stringLocalizer = stringLocalizer;
    }

    private IActionResult RenderError(SignInWidgetViewModel model)
    {
        ModelState.AddModelError(string.Empty, stringLocalizer[SIGN_IN_FAILED]);
        return PartialView("~/Features/Membership/Widgets/SignIn/SignInForm.cshtml", model);
    }

    private IActionResult RenderSuccess(string redirectUrl)
    {
        var model = new SignInWidgetViewModel
        {
            DisplayForm = true,
            AuthenticationSuccessful = true,
            RedirectUrl = redirectUrl

        };
        return PartialView("~/Features/Membership/Widgets/SignIn/SignInForm.cshtml", model);
    }

    [HttpPost($"{{{ApplicationConstants.LANGUAGE_KEY}}}{ApplicationConstants.AUTHENTICATE_ACTION_PATH}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Authenticate(SignInWidgetViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RenderError(model);
        }

        var signInResult = await membershipService.SignIn(model.UserNameOrEmail, model.Password, model.StaySignedIn);

        return signInResult.Succeeded
            ? RenderSuccess(model.RedirectUrl)
            : RenderError(model);
    }

    [Authorize]
    [HttpPost("/Authentication/SignOut")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignOut(SignOutFormModel model)
    {
        await membershipService.SignOut();
        return Redirect(model.RedirectUrl);
    }

    [HttpGet(ApplicationConstants.ACCESS_DENIED_ACTION_PATH)]
    public async Task<IActionResult> AccessDenied([FromQuery(Name = ApplicationConstants.RETURN_URL_PARAMETER)] string returnUrl)
    {
        string language = GetLanguageFromReturnUrl(returnUrl);

        string signInUrl = await membershipService.GetSignInUrl(language);

        string redirectUrl = signInUrl + QueryString.Create(ApplicationConstants.RETURN_URL_PARAMETER, returnUrl);

        return Redirect(redirectUrl);
    }

    private string GetLanguageFromReturnUrl(string returnUrl)
    {
        var languages = contentLanguageInfoProvider.Get()
            .Column(nameof(ContentLanguageInfo.ContentLanguageName));

        foreach (var language in languages)
        {
            if (returnUrl.StartsWith($"/{language.ContentLanguageName}/") || returnUrl.StartsWith($"~/{language.ContentLanguageName}/"))
            {
                return language.ContentLanguageName;
            }
        }
        // Since this controller action has no language in its path, this will return the channel default.
        return preferredLanguageRetriever.Get();
    }
}