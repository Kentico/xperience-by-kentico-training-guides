namespace TrainingGuides.Web.Components.PageTemplates;

public class ArticlePageViewModel
{
    public string Title { get; set; }
    public string Text { get; set; }

    public static ArticlePageViewModel GetViewModel(ArticlePage article)
    {
        if (article == null)
        {
            return new ArticlePageViewModel();
        }

        return new ArticlePageViewModel
        {
            Title = article.ArticlePageContent.FirstOrDefault()?.ArticleTitle,
            Text = article.ArticlePageContent.FirstOrDefault()?.ArticleText
        };
    }

}
