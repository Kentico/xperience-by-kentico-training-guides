using TrainingGuides;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using AspNetCore.Unobtrusive.Ajax;
using Kentico.Activities.Web.Mvc;
using TrainingGuides.Web.Features.DataProtection.Shared;
using TrainingGuides.Web;

string TrainingGuidesAllowSpecificOrigins = "_trainingGuidesAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Enable desired Kentico Xperience features
builder.Services.AddKentico(features =>
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
        }
    });
    features.UseActivityTracking();
    features.UseWebPageRouting();
});

/*
 * All of these service collection extensions/methods are fluent
 * so they _can_ be chained off each other if you don't want
 * to repeat builder.Services over and over
 *
 * The same chainability applies to the middleware registration calls below
 */
builder.Services.Configure<CookieLevelOptions>(options =>
{
    options.CookieConfigurations.Add(CookieNames.COOKIE_CONSENT_LEVEL, CookieLevel.System);
    options.CookieConfigurations.Add(CookieNames.COOKIE_ACCEPTANCE, CookieLevel.System);
});

builder.Services.AddAuthentication();

builder.Services.AddUnobtrusiveAjax();

builder.Services.AddTrainingGuidesServices();

builder.Services.AddControllersWithViews();
builder.Services.AddMvc().AddMvcLocalization();

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

app.InitKentico();
app.UseStaticFiles();
app.UseKentico();

app.UseCookiePolicy();

app.UseAuthentication();

app.Kentico().MapRoutes();

app.UseCors(TrainingGuidesAllowSpecificOrigins);

app.Run();
