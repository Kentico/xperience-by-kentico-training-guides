using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles;

public class ArticlePageViewModel
{
    public string? Title { get; set; }
    public HtmlString? Summary { get; set; }
    public HtmlString? Text { get; set; }
    public AssetViewModel? TeaserImage { get; set; }
    public DateTime CreatedOn { get; set; }
    public List<ArticlePageViewModel> RelatedNews { get; set; } = [];
    public string? Url { get; set; }

    /// <summary>
    /// Creates a new instance of ArticlePageViewModel, setting the properties using ArticlePage given as a parameter.
    /// </summary>
    /// <param name="articlePage">Corresponding Article page object.</param>
    /// <returns>New instance of ArticlePageViewModel.</returns>
    public static ArticlePageViewModel GetViewModel(ArticlePage articlePage)
    {
        var article = articlePage.ArticlePageContent.FirstOrDefault();
        var articleTeaserImage = article?.ArticleTeaser.FirstOrDefault();

        return new ArticlePageViewModel
        {
            Title = article?.ArticleTitle,
            Summary = new HtmlString(article?.ArticleSummary),
            Text = new HtmlString(article?.ArticleText),
            CreatedOn = articlePage.ArticlePagePublishDate,
            TeaserImage = AssetViewModel.GetViewModel(articleTeaserImage!)
        };
    }

    /// <summary>
    /// Sets Url property of this ArticlePageViewModel and returns the same view model instance.
    /// </summary>
    /// <param name="url">String value, URL to be set.</param>
    /// <returns>ArticlePageViewModel with the URL set.</returns>
    public ArticlePageViewModel SetArticlePageUrl(string url)
    {
        Url = url;
        return this;
    }
}
