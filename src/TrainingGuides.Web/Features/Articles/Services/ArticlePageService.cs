using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles.Services;

public class ArticlePageService : IArticlePageService
{
    public ArticlePageService()
    { }

    /// <summary>
    /// Creates a new instance of <see cref="ArticlePageViewModel"/>, setting the properties using ArticlePage given as a parameter.
    /// </summary>
    /// <param name="articlePage">Corresponding Article page object.</param>
    /// <returns>New instance of ArticlePageViewModel.</returns>
    public ArticlePageViewModel GetArticlePageViewModel(ArticlePage? articlePage)
    {
        if (articlePage == null)
        {
            return new ArticlePageViewModel();
        }

        var article = articlePage.ArticlePageContent.FirstOrDefault();
        var articleTeaserImage = article?.ArticleTeaser.FirstOrDefault();

        string articleUrl = GetArticlePageRelativeUrl(articlePage);

        return new ArticlePageViewModel
        {
            Title = article?.ArticleTitle ?? string.Empty,
            SummaryHtml = new HtmlString(article?.ArticleSummary),
            TextHtml = new HtmlString(article?.ArticleText),
            CreatedOn = articlePage.ArticlePagePublishDate,
            TeaserImage = AssetViewModel.GetViewModel(articleTeaserImage),
            Url = articleUrl
        };
    }

    public virtual string GetArticlePageRelativeUrl(ArticlePage articlePage) =>
        articlePage?.GetUrl().RelativePath ?? string.Empty;
}