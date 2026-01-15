using Microsoft.Extensions.DependencyInjection;
using TrainingGuides.Admin.ProductStock;

namespace TrainingGuides.Admin;

public static class ServiceCollectionExtensions
{
    public static void AddTrainingGuidesAdminServices(this IServiceCollection services) =>
        services.AddTransient<IProductMetadataRetriever, ProductMetadataRetriever>();
}