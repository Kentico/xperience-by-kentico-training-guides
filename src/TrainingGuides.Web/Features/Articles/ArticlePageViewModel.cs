namespace TrainingGuides.Web.Features.Articles;

/*
 * This class is only used in the Controller
 * I would recommend moving it to that file since it has tight coupling
 * with the Controller
 */
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
