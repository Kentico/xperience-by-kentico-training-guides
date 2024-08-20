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

    public static ArticlePageViewModel GetViewModel(ArticlePage? articlePage)
    {
        if (articlePage == null)
        {
            return new ArticlePageViewModel();
        }

        var article = articlePage.ArticlePageContent?.FirstOrDefault();
        var articleTeaserImage = article?.ArticleTeaser?.FirstOrDefault();

        return new ArticlePageViewModel
        {
            Title = article?.ArticleTitle,
            Summary = new HtmlString(article?.ArticleSummary),
            Text = new HtmlString(article?.ArticleText),
            CreatedOn = articlePage.ArticlePagePublishDate,
            TeaserImage = AssetViewModel.GetViewModel(articleTeaserImage!)
        };
    }

}
