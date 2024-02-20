using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles.Services;

public class ArticlePageService : IArticlePageService
{
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    public ArticlePageService(IWebPageUrlRetriever webPageUrlRetriever)
    {
        this.webPageUrlRetriever = webPageUrlRetriever;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ArticlePageViewModel"/>, setting the properties using ArticlePage given as a parameter.
    /// </summary>
    /// <param name="articlePage">Corresponding Article page object.</param>
    /// <returns>New instance of ArticlePageViewModel.</returns>
    public async Task<ArticlePageViewModel> GetArticlePageViewModel(ArticlePage articlePage)
    {
        var article = articlePage.ArticlePageContent.FirstOrDefault();
        var articleTeaserImage = article?.ArticleTeaser.FirstOrDefault();

        string articleUrl = (await webPageUrlRetriever.Retrieve(articlePage)).RelativePath;

        return new ArticlePageViewModel
        {
            Title = article?.ArticleTitle,
            Summary = new HtmlString(article?.ArticleSummary),
            Text = new HtmlString(article?.ArticleText),
            CreatedOn = articlePage.ArticlePagePublishDate,
            TeaserImage = AssetViewModel.GetViewModel(articleTeaserImage!),
            Url = articleUrl
        };
    }
}
