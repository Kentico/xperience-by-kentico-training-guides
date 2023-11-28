using TrainingGuides.Web.Services.Content;
using TrainingGuides.Web.Services.Cryptography;
using TrainingGuides.Web.Features.DataProtection.Services;

namespace TrainingGuides.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTrainingGuidesServices(this IServiceCollection services)
    {
        services.AddSingleton<IStringEncryptionService, AesEncryptionService>();
        services.AddSingleton<IFormCollectionService, FormCollectionService>();
        services.AddSingleton<ICookieConsentService, CookieConsentService>();

        services.AddTransient<IContentItemRetrieverService, ContentItemRetrieverService>();
        services.AddTransient(typeof(IContentItemRetrieverService<>), typeof(ContentItemRetrieverService<>));
    }
}
