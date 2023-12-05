using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.Articles;

public class ArticlePageViewModel
{
    public string Title { get; set; }
    public HtmlString Text { get; set; }

    public static ArticlePageViewModel GetViewModel(ArticlePage article)
    {
        if (article == null)
        {
            return new ArticlePageViewModel();
        }

        return new ArticlePageViewModel
        {
            Title = article.ArticlePageContent.FirstOrDefault()?.ArticleTitle,
            Text = new HtmlString(article.ArticlePageContent.FirstOrDefault()?.ArticleText)
        };
    }

}
