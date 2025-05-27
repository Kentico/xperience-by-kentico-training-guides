using AspNetCore.Unobtrusive.Ajax;
using CMS.EmailEngine;
using Kentico.Activities.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.EmailBuilder.Web.Mvc;
using Kentico.Membership;

// using Kentico.CrossSiteTracking.Web.Mvc;
// using Kentico.OnlineMarketing.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using Kentico.Xperience.Mjml;
using Kentico.Xperience.Mjml.StarterKit.Rcl;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Sections;
using Microsoft.AspNetCore.Identity;
using TrainingGuides;
using TrainingGuides.Web;
using TrainingGuides.Web.Features.DataProtection.Shared;
using TrainingGuides.Web.Features.Membership;
using TrainingGuides.Web.Features.Shared.Helpers;
//using TrainingGuides.Web.Features.Shared.Helpers.Startup;

// Functionality related to cross-site tracking is currently disabled while we investigate an issue (#85 on GitHub)
// string trainingGuidesAllowSpecificOrigins = "_trainingGuidesAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Functionality related to cross-site tracking is currently disabled while we investigate an issue (#85 on GitHub)
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(name: trainingGuidesAllowSpecificOrigins,
//         policy =>
//         {
//             policy
//             .WithOrigins("https://The-URL-of-your-external-site.com")
//             .WithHeaders("content-type")
//             .AllowAnyMethod()
//             .AllowCredentials();
//         });
// });

// Enable desired Kentico Xperience features
builder.Services.AddKentico(async features =>
{
    features.UsePageBuilder(new PageBuilderOptions
    {
        DefaultSectionIdentifier = ComponentIdentifiers.Sections.SINGLE_COLUMN,
        RegisterDefaultSection = false,
        ContentTypeNames = new[] {
            LandingPage.CONTENT_TYPE_NAME,
            ArticlePage.CONTENT_TYPE_NAME,
            DownloadsPage.CONTENT_TYPE_NAME,
            EmptyPage.CONTENT_TYPE_NAME,
            ProductPage.CONTENT_TYPE_NAME,
            ProfilePage.CONTENT_TYPE_NAME
        }
    });
    // Functionality related to cross-site tracking is currently disabled while we investigate an issue (#85 on GitHub)
    // features.UseCrossSiteTracking(
    //     new CrossSiteTrackingOptions
    //     {
    //         ConsentSettings = new[] {
    //             new CrossSiteTrackingConsentOptions
    //             {
    //                 WebsiteChannelName = "TrainingGuidesPages",
    //                 ConsentName = await StartupHelper.GetMarketingConsentCodeName(),
    //                 AgreeCookieLevel = CookieLevel.All.Level
    //             }
    //         },
    //     });
    features.UseActivityTracking();
    features.UseWebPageRouting(new WebPageRoutingOptions { LanguageNameRouteValuesKey = ApplicationConstants.LANGUAGE_KEY });
    features.UseEmailBuilder();
});

builder.Services.AddXperienceSmtp(options =>
{
    options.Server = new SmtpServer { Host = "localhost", Port = 25 };
});

builder.Services.Configure<EmailBuilderOptions>(options =>
{
    // Allows Email Builder for the 'Acme.Email' content type
    options.AllowedEmailContentTypeNames = ["TrainingGuides.BasicEmail"];
    // Replaces the default Email Builder section
    options.RegisterDefaultSection = false;
    options.DefaultSectionIdentifier = FullWidthEmailSection.IDENTIFIER;
});

builder.Services.AddMjmlForEmails();

// Configure the Starter Kit options
builder.Services.AddKenticoMjmlStarterKit(options =>
{
    // Enter the path to your email CSS stylesheet, starting from the root of your project's 'wwwroot' folder
    //options.StyleSheetPath = "EmailBuilder.css";
    // Enter the code names of content types representing image assets in your project
    // Required for the Image widget
    options.AllowedImageContentTypes = [Asset.CONTENT_TYPE_NAME, GalleryImage.CONTENT_TYPE_NAME];
    // Enter the code names of page content types representing products in your project
    // Required for the Product widget
    options.AllowedProductContentTypes = [ProductPage.CONTENT_TYPE_NAME];
});

builder.Services.Configure<CookieLevelOptions>(options =>
{
    options.CookieConfigurations.Add(CookieNames.COOKIE_CONSENT_LEVEL, CookieLevel.System);
    options.CookieConfigurations.Add(CookieNames.COOKIE_ACCEPTANCE, CookieLevel.System);
    options.CookieConfigurations.Add(CookieNames.COOKIE_AUTHENTICATION, CookieLevel.Essential);
});

builder.Services
    .AddIdentity<GuidesMember, NoOpApplicationRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddUserStore<ApplicationUserStore<GuidesMember>>()
    .AddRoleStore<NoOpApplicationRoleStore>()
    .AddUserManager<UserManager<GuidesMember>>()
    .AddSignInManager<SignInManager<GuidesMember>>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = new PathString(ApplicationConstants.ACCESS_DENIED_ACTION_PATH);
    options.ReturnUrlParameter = ApplicationConstants.RETURN_URL_PARAMETER;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = CookieNames.COOKIE_AUTHENTICATION;
});

builder.Services.AddAuthorization();

builder.Services.AddAuthentication();

builder.Services.AddUnobtrusiveAjax();

builder.Services.AddTrainingGuidesServices();
builder.Services.AddTrainingGuidesOptions();

builder.Services.AddControllersWithViews(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddMvc()
    .AddMvcLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResources));
    });

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

app.InitKentico();
app.UseStaticFiles();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseKentico();
app.UseAuthorization();

app.Kentico().MapRoutes();

// Functionality related to cross-site tracking is currently disabled while we investigate an issue (#85 on GitHub)
// app.UseCors(trainingGuidesAllowSpecificOrigins);

app.Run();
