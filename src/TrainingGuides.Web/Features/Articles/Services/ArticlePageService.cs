using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Shared.Helpers;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles.Services;

public class ArticlePageService : IArticlePageService
{
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    private readonly IServiceProvider serviceProvider;
    private readonly IStringLocalizer<SharedResources> stringLocalizer;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    public ArticlePageService(IWebPageUrlRetriever webPageUrlRetriever,
        IServiceProvider serviceProvider,
        IStringLocalizer<SharedResources> stringLocalizer,
        IPreferredLanguageRetriever preferredLanguageRetriever)
    {
        this.webPageUrlRetriever = webPageUrlRetriever;
        this.serviceProvider = serviceProvider;
        this.stringLocalizer = stringLocalizer;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
    }


    ///  <inheritdoc/>
    public async Task<ArticlePageViewModel> GetArticlePageViewModel(ArticlePage? articlePage)
    {
        if (articlePage == null)
        {
            return new ArticlePageViewModel();
        }

        string articleUrl = (await webPageUrlRetriever.Retrieve(articlePage, preferredLanguageRetriever.Get())).RelativePath;
        var articleSchema = articlePage.ArticlePageArticleContent.FirstOrDefault();

        if (articleSchema != null)
        {
            var articleSchemaTeaserImage = articleSchema.ArticleSchemaTeaser.FirstOrDefault();

            return new ArticlePageViewModel
            {
                Title = articleSchema.ArticleSchemaTitle,
                Summary = new HtmlString(articleSchema?.ArticleSchemaSummary),
                Text = new HtmlString(articleSchema?.ArticleSchemaText),
                CreatedOn = articlePage.ArticlePagePublishDate,
                TeaserImage = AssetViewModel.GetViewModel(articleSchemaTeaserImage!),
                Url = articleUrl
            };
        }

        var article = articlePage.ArticlePageContent.FirstOrDefault();
        var articleTeaserImage = article?.ArticleTeaser.FirstOrDefault();

        return new ArticlePageViewModel
        {
            Title = article?.ArticleTitle ?? string.Empty,
            Summary = new HtmlString(article?.ArticleSummary),
            Text = new HtmlString(article?.ArticleText),
            CreatedOn = articlePage.ArticlePagePublishDate,
            TeaserImage = AssetViewModel.GetViewModel(articleTeaserImage!),
            Url = articleUrl
        };
    }

    /// <inheritdoc/>
    public async Task<ArticlePageViewModel> GetArticlePageViewModelWithSecurity(ArticlePage? articlePage)
    {
        var originalViewModel = await GetArticlePageViewModel(articlePage);

        using (var scope = serviceProvider.CreateScope())
        {
            var membershipService = scope.ServiceProvider.GetRequiredService<IMembershipService>();

            if (articlePage is not null
                && articlePage.SystemFields.ContentItemIsSecured
                && !await membershipService.IsMemberAuthenticated())
            {
                string signInUrl = await membershipService.GetSignInUrl(preferredLanguageRetriever.Get());

                string signInUrlWithReturn = signInUrl + QueryString.Create(ApplicationConstants.RETURN_URL_PARAMETER, originalViewModel.Url.Replace("~", "")).ToString();

                var message = new HtmlString(stringLocalizer["Sign in to view this content."]);
                return new ArticlePageViewModel
                {
                    Title = $"{stringLocalizer["(🔒 Locked)"]} {originalViewModel.Title}",
                    Summary = message,
                    Text = message,
                    CreatedOn = articlePage.ArticlePagePublishDate,
                    TeaserImage = originalViewModel.TeaserImage,
                    Url = signInUrlWithReturn
                };
            }
        }
        return originalViewModel;
    }
}
