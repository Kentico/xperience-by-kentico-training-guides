using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Membership.Widgets.SignIn;
using TrainingGuides.Web.Features.Membership.Widgets.LinkOrSignOut;
using TrainingGuides.Web.Features.Shared.Helpers;
using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc.Routing;

namespace TrainingGuides.Web.Features.Membership.Controllers;



public class AuthenticationController : Controller
{
    private readonly IMembershipService membershipService;
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    private readonly IWebsiteChannelContext websiteChannelContext;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    private const string SIGN_IN_FAILED = "Your sign-in attempt was not successful. Please try again.";

    public AuthenticationController(IMembershipService membershipService,
        IWebPageUrlRetriever webPageUrlRetriever,
        IWebsiteChannelContext websiteChannelContext,
        IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.membershipService = membershipService;
        this.webPageUrlRetriever = webPageUrlRetriever;
        this.websiteChannelContext = websiteChannelContext;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }

    private IActionResult RenderError(SignInWidgetViewModel model)
    {
        ModelState.AddModelError(string.Empty, SIGN_IN_FAILED);
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

    [HttpGet(ApplicationConstants.ACCESS_DENIED_CONTROLLER_PATH)]
    public async Task<IActionResult> AccessDenied([FromQuery(Name = ApplicationConstants.RETURN_URL_PARAMETER)] string returnUrl)
    {
        var signInUrl = await webPageUrlRetriever.Retrieve(
            webPageTreePath: ApplicationConstants.EXPECTED_SIGN_IN_PATH,
            websiteChannelName: websiteChannelContext.WebsiteChannelName,
            languageName: preferredLanguageRetriever.Get()
        );

        string redirectUrl = signInUrl.RelativePath + QueryString.Create(ApplicationConstants.RETURN_URL_PARAMETER, returnUrl);

        return Redirect(redirectUrl);
    }
}