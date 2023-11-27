using TrainingGuides.Web.Services.Content;
using TrainingGuides.Web.Services.Cryptography;
using TrainingGuides.Web.DataProtection;

namespace TrainingGuides.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTrainingGuidesServices(this IServiceCollection services)
    {
        services.AddSingleton<IStringEncryptionService, AesEncryptionService>();
        services.AddSingleton<CurrentContactIsTrackableService>();
        services.AddTransient(typeof(IContentItemRetrieverService<>), typeof(ContentItemRetrieverService<>));
        services.AddTransient(typeof(IContentItemRetrieverService), typeof(ContentItemRetrieverService));
    }
}
