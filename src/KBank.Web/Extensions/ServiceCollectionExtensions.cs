using KBank.Web.DataProtection;
using KBank.Web.PageTemplates;
using KBank.Web.Services.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace KBank.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddKBankServices(this IServiceCollection services)
    {
        services.AddSingleton<IStringEncryptionService,AesEncryptionService>();
        services.AddSingleton<CurrentContactIsTrackableService>();
    }
	
	public static void AddKBankPageTemplateServices(this IServiceCollection services)
    {
        services.AddSingleton<HeadingAndSubPageTemplateService>();
        services.AddSingleton<DownloadPagePageTemplateService>();
        services.AddSingleton<HomePagePageTemplateService>();
    }
}