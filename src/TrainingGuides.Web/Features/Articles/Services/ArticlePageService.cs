using CMS.ContentEngine;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Shared.Helpers;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Articles.Services;

public class ArticlePageService : IArticlePageService
{
    private readonly IStringLocalizer<SharedResources> stringLocalizer;
    private readonly IHttpRequestService httpRequestService;
    public ArticlePageService(
        IStringLocalizer<SharedResources> stringLocalizer,
        IHttpRequestService httpRequestService)
    {
        this.stringLocalizer = stringLocalizer;
        this.httpRequestService = httpRequestService;
    }

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
                IsSecured = articlePage.SystemFields.ContentItemIsSecured
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
            IsSecured = articlePage.SystemFields.ContentItemIsSecured
        };
    }

    public virtual string GetArticlePageRelativeUrl(ArticlePage articlePage) =>
        articlePage?.GetUrl().RelativePath ?? string.Empty;

    /// <inheritdoc/>
    public ArticlePageViewModel GetArticlePageViewModelWithSecurity(ArticlePage? articlePage, string signInUrl, bool isAuthenticated)
    {
        var originalViewModel = GetArticlePageViewModel(articlePage);

        if (articlePage is null)
        {
            return originalViewModel;
        }

        bool reusableArticleSecured = IsReusableArticleSecured(articlePage);

        if ((articlePage.SystemFields.ContentItemIsSecured
            || reusableArticleSecured)
            && !isAuthenticated)
        {
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
                IsSecured = articlePage.SystemFields.ContentItemIsSecured
            };
        }
        return originalViewModel;
    }

    /// <inheritdoc/>
    public bool IsReusableArticleSecured(ArticlePage articlePage)
    {
        var oldArticle = articlePage.ArticlePageContent.FirstOrDefault();
        var newArticle = (IContentItemFieldsSource?)articlePage.ArticlePageArticleContent.FirstOrDefault();

        return (oldArticle?.SystemFields.ContentItemIsSecured ?? false)
            || (newArticle?.SystemFields.ContentItemIsSecured ?? false);
    }
}
