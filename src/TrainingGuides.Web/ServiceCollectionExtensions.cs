using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web;

public static class ServiceCollectionExtensions
{
    public static void AddTrainingGuidesServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpRequestService, HttpRequestService>();
        services.AddSingleton<IArticlePageService, ArticlePageService>();
        services.AddSingleton<IContentItemRetrieverService, ContentItemRetrieverService>();

        services.AddTransient(typeof(IContentItemRetrieverService<>), typeof(ContentItemRetrieverService<>));
    }
}
