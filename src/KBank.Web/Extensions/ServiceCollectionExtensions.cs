using KBank.Web.PageTemplates;
using Microsoft.Extensions.DependencyInjection;

namespace KBank.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddKBankServices(this IServiceCollection services)
    {

    }
	
	public static void AddKBankPageTemplateServices(this IServiceCollection services)
    {
        services.AddSingleton<HeadingAndSubPageTemplateService>();
        services.AddSingleton<DownloadPagePageTemplateService>();
        services.AddSingleton<HomePagePageTemplateService>();
    }
}