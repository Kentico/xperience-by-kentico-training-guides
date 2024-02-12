namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetViewModel
{
    public IEnumerable<ArticlePageViewModel>? Articles { get; set; }
    public string? CtaText { get; set; }
}