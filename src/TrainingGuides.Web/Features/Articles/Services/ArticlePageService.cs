using CMS.ContentEngine;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Shared.Helpers;
using TrainingGuides.Web.Features.Shared.Models;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Articles.Services;

public class ArticlePageService : IArticlePageService
{
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    private readonly IStringLocalizer<SharedResources> stringLocalizer;
    private readonly IPreferredLanguageRetriever preferredLanguageRetriever;
    private readonly IHttpRequestService httpRequestService;
    public ArticlePageService(IWebPageUrlRetriever webPageUrlRetriever,
        IStringLocalizer<SharedResources> stringLocalizer,
        IPreferredLanguageRetriever preferredLanguageRetriever,
        IHttpRequestService httpRequestService)
    {
        this.webPageUrlRetriever = webPageUrlRetriever;
        this.stringLocalizer = stringLocalizer;
        this.preferredLanguageRetriever = preferredLanguageRetriever;
        this.httpRequestService = httpRequestService;
    }


    ///  <inheritdoc/>
    public async Task<ArticlePageViewModel> GetArticlePageViewModel(ArticlePage? articlePage)
    {
        if (articlePage == null)
        {
            return new ArticlePageViewModel();
        }

        string language = preferredLanguageRetriever.Get();

        string articleUrl = (await webPageUrlRetriever.Retrieve(articlePage, language)).RelativePath;
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
                Url = articleUrl,
                IsSecured = articlePage.SystemFields.ContentItemIsSecured
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
            Url = articleUrl,
            IsSecured = articlePage.SystemFields.ContentItemIsSecured
        };
    }

    /// <inheritdoc/>
    public async Task<ArticlePageViewModel> GetArticlePageViewModelWithSecurity(ArticlePage? articlePage, string signInUrl, bool isAuthenticated)
    {
        var originalViewModel = await GetArticlePageViewModel(articlePage);

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
                Summary = message,
                Text = messageWithLink,
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
