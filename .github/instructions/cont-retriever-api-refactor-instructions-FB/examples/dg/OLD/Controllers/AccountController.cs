﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using CMS.Core;
using CMS.DataEngine;
using CMS.Websites;
using CMS.Websites.Routing;

using DancingGoat.Models;

using Kentico.Content.Web.Mvc.Routing;
using Kentico.Membership;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace DancingGoat.Controllers
{
    public class AccountController : Controller
    {
        private readonly IStringLocalizer<SharedResources> localizer;
        private readonly IEventLogService eventLogService;
        private readonly IInfoProvider<WebsiteChannelInfo> websiteChannelProvider;
        private readonly IWebPageUrlRetriever webPageUrlRetriever;
        private readonly IWebsiteChannelContext websiteChannelContext;
        private readonly IPreferredLanguageRetriever currentLanguageRetriever;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<SharedResources> localizer,
            IEventLogService eventLogService,
            IInfoProvider<WebsiteChannelInfo> websiteChannelProvider,
            IWebPageUrlRetriever webPageUrlRetriever,
            IWebsiteChannelContext websiteChannelContext,
            IPreferredLanguageRetriever preferredLanguageRetriever)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.localizer = localizer;
            this.eventLogService = eventLogService;
            this.websiteChannelProvider = websiteChannelProvider;
            this.webPageUrlRetriever = webPageUrlRetriever;
            this.websiteChannelContext = websiteChannelContext;
            this.currentLanguageRetriever = preferredLanguageRetriever;
        }


        // GET: Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }


        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var signInResult = SignInResult.Failed;

            try
            {
                signInResult = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.StaySignedIn, false);
            }
            catch (Exception ex)
            {
                eventLogService.LogException("AccountController", "Login", ex);
            }

            if (signInResult.Succeeded)
            {
                var decodedReturnUrl = WebUtility.UrlDecode(returnUrl);
                if (!string.IsNullOrEmpty(decodedReturnUrl) && Url.IsLocalUrl(decodedReturnUrl))
                {
                    return Redirect(decodedReturnUrl);
                }

                return Redirect(await GetHomeWebPageUrl(cancellationToken));
            }

            ModelState.AddModelError(string.Empty, localizer["Your sign-in attempt was not successful. Please try again."].ToString());

            return View(model);
        }


        // POST: Account/Logout 
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Logout(CancellationToken cancellationToken = default)
        {
            await signInManager.SignOutAsync();
            return Redirect(await GetHomeWebPageUrl(cancellationToken));
        }


        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }


        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var member = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                Enabled = true
            };

            var registerResult = new IdentityResult();

            try
            {
                registerResult = await userManager.CreateAsync(member, model.Password);
            }
            catch (Exception ex)
            {
                eventLogService.LogException("AccountController", "Register", ex);
                ModelState.AddModelError(string.Empty, localizer["Your registration was not successful."]);
            }

            if (registerResult.Succeeded)
            {
                var signInResult = await signInManager.PasswordSignInAsync(member, model.Password, true, false);

                if (signInResult.Succeeded)
                {
                    return Redirect(await GetHomeWebPageUrl(cancellationToken));
                }
            }

            foreach (var error in registerResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        private async Task<string> GetHomeWebPageUrl(CancellationToken cancellationToken)
        {
            var websiteChannelId = websiteChannelContext.WebsiteChannelID;
            var websiteChannel = await websiteChannelProvider.GetAsync(websiteChannelId, cancellationToken);

            if (websiteChannel == null)
            {
                return string.Empty;
            }

            var homePageUrl = await webPageUrlRetriever.Retrieve(
                websiteChannel.WebsiteChannelHomePage,
                websiteChannelContext.WebsiteChannelName,
                currentLanguageRetriever.Get(),
                websiteChannelContext.IsPreview,
                cancellationToken
            );

            if (string.IsNullOrEmpty(homePageUrl?.RelativePath))
            {
                return "/";
            }

            return homePageUrl.RelativePath;

        }
    }
}
