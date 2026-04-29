using CMS.ContentEngine;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Membership.Services;
using TrainingGuides.Web.Features.Shared.Helpers;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Articles.Services;

public class ArticlePageService(
    IStringLocalizer<SharedResources> stringLocalizer,
    IHttpRequestService httpRequestService,
    IMembershipService membershipService,
    IPreferredLanguageRetriever preferredLanguageRetriever) : IArticlePageService
{

    ///  <inheritdoc/>
    public ArticlePageViewModel GetArticlePageViewModel(ArticlePage? articlePage)
    {
        if (articlePage == null)
        {
            return new ArticlePageViewModel();
        }

        string articleUrl = GetArticlePageRelativeUrl(articlePage);
        var articleSchema = articlePage.ArticlePageArticleContent.FirstOrDefault();

        if (articleSchema != null)
        {
            var articleSchemaTeaserImage = articleSchema.ArticleSchemaTeaser.FirstOrDefault();

            return new ArticlePageViewModel
            {
                Title = articleSchema.ArticleSchemaTitle,
                SummaryHtml = new HtmlString(articleSchema?.ArticleSchemaSummary),
                TextHtml = new HtmlString(articleSchema?.ArticleSchemaText),
                CreatedOn = articlePage.ArticlePagePublishDate,
                TeaserImage = AssetViewModel.GetViewModel(articleSchemaTeaserImage),
                Url = articleUrl,
                Restricted = !CanCurrentUserAccessArticlePage(articlePage),
                RequiresSignIn = false
            };
        }

        var article = articlePage.ArticlePageContent.FirstOrDefault();
        var articleTeaserImage = article?.ArticleTeaser.FirstOrDefault();

        return new ArticlePageViewModel
        {
            Title = article?.ArticleTitle ?? string.Empty,
            SummaryHtml = new HtmlString(article?.ArticleSummary),
            TextHtml = new HtmlString(article?.ArticleText),
            CreatedOn = articlePage.ArticlePagePublishDate,
            TeaserImage = AssetViewModel.GetViewModel(articleTeaserImage),
            Url = articleUrl,
            Restricted = !CanCurrentUserAccessArticlePage(articlePage),
            RequiresSignIn = false
        };
    }

    public virtual string GetArticlePageRelativeUrl(ArticlePage articlePage) =>
        articlePage?.GetUrl().RelativePath ?? string.Empty;

    /// <inheritdoc/>
    public async Task<ArticlePageViewModel> GetArticlePageViewModelWithSecurity(ArticlePage? articlePage)
    {
        var originalViewModel = GetArticlePageViewModel(articlePage);

        if (articlePage is null)
        {
            return originalViewModel;
        }

        bool userHasAccess = CanCurrentUserAccessArticlePage(articlePage);

        if (userHasAccess)
        {
            return originalViewModel;
        }

        bool isAuthenticated = await membershipService.IsMemberAuthenticated();

        // If not authenticated, show sign-in prompt
        if (!isAuthenticated)
        {
            string language = preferredLanguageRetriever.Get();
            string signInUrl = await membershipService.GetSignInUrl(language);
            string relativePath = originalViewModel.Url.TrimStart('~');

            string baseUrl = httpRequestService.GetBaseUrl();

            var signInUri = new UriBuilder(baseUrl)
            {
                Path = signInUrl.TrimStart('~'),
                Query = QueryString.Create(ApplicationConstants.RETURN_URL_PARAMETER, relativePath).ToString()
            };

            string messageWithLinkString = $"<a href=\"{signInUri}\">{stringLocalizer["Sign in"]}</a> {stringLocalizer["to view this content."]}";

            var message = new HtmlString(stringLocalizer["Sign in to view this content."]);
            var messageWithLink = new HtmlString(messageWithLinkString);

            return new ArticlePageViewModel
            {
                Title = $"{stringLocalizer["(🔒 Locked)"]} {originalViewModel.Title}",
                SummaryHtml = message,
                TextHtml = messageWithLink,
                CreatedOn = articlePage.ArticlePagePublishDate,
                TeaserImage = originalViewModel.TeaserImage,
                Url = signInUri.ToString(),
                Restricted = true,
                RequiresSignIn = true
            };
        }
        // If authenticated but no access, show access denied message
        else
        {
            string deniedReturnPath = originalViewModel.Url.TrimStart('~');
            string accessDeniedUrl = $"{ApplicationConstants.ACCESS_DENIED_ACTION_PATH}{QueryString.Create(ApplicationConstants.RETURN_URL_PARAMETER, deniedReturnPath)}";

            var accessDeniedMessage = new HtmlString(stringLocalizer["You do not have permission to access this content. Upgrade to our higher tier."]);
            return new ArticlePageViewModel
            {
                Title = $"{stringLocalizer["(🔒 Locked)"]} {originalViewModel.Title}",
                SummaryHtml = accessDeniedMessage,
                TextHtml = accessDeniedMessage,
                CreatedOn = articlePage.ArticlePagePublishDate,
                TeaserImage = originalViewModel.TeaserImage,
                Url = accessDeniedUrl,
                Restricted = true,
                RequiresSignIn = false
            };
        }
    }

    /// <inheritdoc/>
    public bool CanCurrentUserAccessArticlePage(ArticlePage articlePage)
    {
        bool pageAccessible = membershipService.CanCurrentUserAccessContentItem(articlePage);

        if (!pageAccessible)
            return false;

        bool oldArticleAccessible = articlePage.ArticlePageContent
            .Any(article => membershipService.CanCurrentUserAccessContentItem(article));

        bool newArticleAccessible = articlePage.ArticlePageArticleContent
            .OfType<IContentItemFieldsSource>()
            .Any(article => membershipService.CanCurrentUserAccessContentItem(article));

        bool articleAccessible = oldArticleAccessible || newArticleAccessible;

        return pageAccessible && articleAccessible;
    }

}
