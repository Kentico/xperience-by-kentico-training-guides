using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web;

public static class ServiceCollectionExtensions
{
    public static void AddTrainingGuidesServices(this IServiceCollection services) =>
        services.AddTransient(typeof(IContentItemRetrieverService<>), typeof(ContentItemRetrieverService<>));
}
