using TrainingGuides.Web.Components.PageTemplates;
using TrainingGuides.Web.Services.Content;

namespace TrainingGuides.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTrainingGuidesServices(this IServiceCollection services)
    {
        services.AddTransient(typeof(IContentItemRetrieverService<>), typeof(ContentItemRetrieverService<>));
    }

    public static void AddTrainingGuidesPageTemplateServices(this IServiceCollection services)
    {
        services.AddSingleton<ArticlePagePageTemplateService>();
        services.AddSingleton<DownloadsPagePageTemplateService>();
        services.AddSingleton<LandingPagePageTemplateService>();
    }
}
