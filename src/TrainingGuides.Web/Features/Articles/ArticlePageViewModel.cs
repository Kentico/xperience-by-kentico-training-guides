using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles;

public class ArticlePageViewModel
{
    public string? Title { get; set; }
    public HtmlString? Text { get; set; }
    public AssetViewModel? Teaser { get; set; }

    public static ArticlePageViewModel GetViewModel(ArticlePage articlePage)
    {
        if (articlePage == null)
        {
            return new ArticlePageViewModel();
        }

        var article = articlePage.ArticlePageContent.FirstOrDefault();
        var articleTeaserImage = article?.ArticleTeaser.FirstOrDefault();

        return new ArticlePageViewModel
        {
            Title = article?.ArticleTitle,
            Text = new HtmlString(article?.ArticleText),
            Teaser = AssetViewModel.GetViewModel(articleTeaserImage!)
        };
    }

}
