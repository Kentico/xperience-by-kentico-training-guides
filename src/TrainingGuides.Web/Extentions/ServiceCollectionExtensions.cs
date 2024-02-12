using TrainingGuides.Web.Features.DataProtection.Services;
using TrainingGuides.Web.Features.HTML.Services;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTrainingGuidesServices(this IServiceCollection services)
    {
        services.AddSingleton<IStringEncryptionService, AesEncryptionService>();
        services.AddSingleton<IFormCollectionService, FormCollectionService>();
        services.AddSingleton<ICookieConsentService, CookieConsentService>();
        services.AddSingleton<IContentItemRetrieverService, ContentItemRetrieverService>();
        services.AddSingleton<IHttpRequestService, HttpRequestService>();

        services.AddScoped<IHeadTagStoreService, HeadTagStoreService>();

        services.AddTransient(typeof(IContentItemRetrieverService<>), typeof(ContentItemRetrieverService<>));
    }
}
