using Kentico.Content.Web.Mvc.Routing;
using KBank.Web.Extensions;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);


// Enable desired Kentico Xperience features
builder.Services.AddKentico(features =>
{
    features.UsePageBuilder();
    features.UsePageRouting();
});

builder.Services.AddAuthentication();

builder.Services.AddKBankServices();
builder.Services.AddKBankPageTemplateServices();

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

app.Run();