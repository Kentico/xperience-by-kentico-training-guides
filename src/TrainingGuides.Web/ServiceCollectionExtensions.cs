using CMS.Commerce;
using TrainingGuides.Web.Commerce.Products.Services;
using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.Commerce.Products.Services;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web;

public static class ServiceCollectionExtensions
{
    public static void AddTrainingGuidesServices(this IServiceCollection services)
    {
        services.AddSingleton<IContentTypeService, ContentTypeService>();
        services.AddSingleton<IHttpRequestService, HttpRequestService>();
        services.AddSingleton<IArticlePageService, ArticlePageService>();
        services.AddSingleton<IContentItemRetrieverService, ContentItemRetrieverService>();

        services.AddScoped<IProductService, ProductService>();

        services.AddTransient(typeof(IProductDataRetriever<,>), typeof(TrainingGuidesProductDataRetriever<,>));
    }
}
