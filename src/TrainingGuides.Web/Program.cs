using TrainingGuides;
using TrainingGuides.Web.Components;
using TrainingGuides.Web.Extensions;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.PageBuilder.Web.Mvc;
using TrainingGuides.Web.Helpers.Cookies;
using Kentico.Web.Mvc;
using AspNetCore.Unobtrusive.Ajax;
using Kentico.Activities.Web.Mvc;

string TrainingGuidesAllowSpecificOrigins = "_trainingGuidesAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

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
        }
    });
    features.UseActivityTracking();
    features.UseWebPageRouting();
});

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
