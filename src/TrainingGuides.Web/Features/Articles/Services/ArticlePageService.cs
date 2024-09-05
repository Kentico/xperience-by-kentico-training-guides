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
    public async Task<ArticlePageViewModel> GetArticlePageViewModel(ArticlePage? articlePage)
    {
        if (articlePage == null)
        {
            return new ArticlePageViewModel();
        }

        string articleUrl = (await webPageUrlRetriever.Retrieve(articlePage)).RelativePath;

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
}
