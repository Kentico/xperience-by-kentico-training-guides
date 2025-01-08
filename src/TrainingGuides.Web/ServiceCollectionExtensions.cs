using TrainingGuides.Web.Features.Articles.Services;
using TrainingGuides.Web.Features.DataProtection.Services;
using TrainingGuides.Web.Features.EmailNotifications;
using TrainingGuides.Web.Features.Html.Services;
using TrainingGuides.Web.Features.Membership.Profile;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Products.Services;
using TrainingGuides.Web.Features.SEO;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web;

public static class ServiceCollectionExtensions
{
    public static void AddTrainingGuidesServices(this IServiceCollection services)
    {
        services.AddSingleton<IStringEncryptionService, AesEncryptionService>();
        services.AddSingleton<IFormCollectionService, FormCollectionService>();
        services.AddSingleton<ICookieConsentService, CookieConsentService>();
        services.AddSingleton<IContentItemRetrieverService, ContentItemRetrieverService>();
        services.AddSingleton<IHttpRequestService, HttpRequestService>();
        services.AddSingleton<IArticlePageService, ArticlePageService>();
        services.AddSingleton<IProductPageService, ProductPageService>();
        services.AddSingleton<IComponentStyleEnumService, ComponentStyleEnumService>();
        services.AddSingleton<IEmailNotificationService, EmailNotificationService>();
        services.AddSingleton<IMemberContactService, MemberContactService>();
        services.AddSingleton<IUpdateProfileService, UpdateProfileService>();

        services.AddScoped<IMembershipService, MembershipService>();
        services.AddScoped<IHeadTagStoreService, HeadTagStoreService>();

        services.AddTransient(typeof(IContentItemRetrieverService<>), typeof(ContentItemRetrieverService<>));

    }

    public static void AddTrainingGuidesOptions(this IServiceCollection services)
    {
        services.ConfigureOptions<EmailNotificationOptionsSetup>();
        services.ConfigureOptions<RobotsOptionsSetup>();
    }
}
