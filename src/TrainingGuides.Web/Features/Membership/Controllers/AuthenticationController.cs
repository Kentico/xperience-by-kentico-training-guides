using CMS.ContentEngine;
using CMS.DataEngine;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;
using TrainingGuides.Web.Features.Membership.Widgets.SignIn;
using TrainingGuides.Web.Features.Shared.Helpers;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Membership.Controllers;

public class AuthenticationController : Controller
{
    private readonly IMembershipService membershipService;
    private readonly IStringLocalizer<SharedResources> stringLocalizer;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider;
    private readonly IHttpRequestService httpRequestService;

    private const string SIGN_IN_FAILED = "Your sign-in attempt was not successful. Please try again.";

    public AuthenticationController(IMembershipService membershipService,
        IStringLocalizer<SharedResources> stringLocalizer,
        IPreferredLanguageRetriever preferredLanguageRetriever,
        IInfoProvider<ContentLanguageInfo> contentLanguageInfoProvider,
        IHttpRequestService httpRequestService)
    {
        this.membershipService = membershipService;
        this.stringLocalizer = stringLocalizer;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
        this.contentLanguageInfoProvider = contentLanguageInfoProvider;
        this.httpRequestService = httpRequestService;
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
    public async Task<IActionResult> Authenticate(SignInWidgetViewModel model, [FromQuery(Name = ApplicationConstants.RETURN_URL_PARAMETER)] string returnUrl)
    {
        if (!ModelState.IsValid)
        {
            return RenderError(model);
        }

        var signInResult = await membershipService.SignIn(model.UserNameOrEmail, model.Password, model.StaySignedIn);

        string returnPath = string.IsNullOrWhiteSpace(returnUrl)
            ? (model.DefaultRedirectPageGuid == Guid.Empty
                ? "/"
                : (await httpRequestService.GetPageRelativeUrl(model.DefaultRedirectPageGuid, preferredLanguageRetriever.Get())).TrimStart('~'))
            : EnsureRelativeReturnUrl(returnUrl);

        string absoluteReturnUrl = httpRequestService.GetAbsoluteUrlForPath(returnPath, false);

        return signInResult.Succeeded
            ? RenderSuccess(absoluteReturnUrl)
            : RenderError(model);
    }

    [Authorize]
    [HttpPost("/Authentication/SignOut")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignOut(SignOutFormModel model)
    {
        await membershipService.SignOut();

        string redirectPath = EnsureRelativeReturnUrl(model.RedirectUrl);

        string redirectUrl = httpRequestService.GetAbsoluteUrlForPath(redirectPath, false);

        return Redirect(redirectUrl);
    }

    [HttpGet(ApplicationConstants.ACCESS_DENIED_ACTION_PATH)]
    public async Task<IActionResult> AccessDenied([FromQuery(Name = ApplicationConstants.RETURN_URL_PARAMETER)] string returnUrl)
    {
        string language = GetLanguageFromReturnUrl(returnUrl);

        string signInUrl = await membershipService.GetSignInUrl(language);

        var query = QueryString.Create(ApplicationConstants.RETURN_URL_PARAMETER, returnUrl);

        var redirectUrl = new UriBuilder(signInUrl)
        {
            Query = query.ToString()
        };

        return Redirect(redirectUrl.ToString());
    }

    private string GetLanguageFromReturnUrl(string returnUrl)
    {
        // Cache this in real-world scenarios
        var languages = contentLanguageInfoProvider.Get()
            .Column(nameof(ContentLanguageInfo.ContentLanguageName))
            .GetListResult<string>();

        foreach (string language in languages)
        {
            if (returnUrl.StartsWith($"/{language}/", StringComparison.OrdinalIgnoreCase) || returnUrl.StartsWith($"~/{language}/", StringComparison.OrdinalIgnoreCase))
            {
                return language;
            }
        }
        // Since this controller action has no language in its path, this will return the channel default.
        return preferredLanguageRetriever.Get();
    }

    private string EnsureRelativeReturnUrl(string returnUrl) =>
        returnUrl.StartsWith('/') || returnUrl.StartsWith("~/")
            ? returnUrl
            : httpRequestService.ExtractRelativePath(returnUrl);
}