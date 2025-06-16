using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;

namespace TrainingGuides.Web.Features.Articles.EmailWidgets;

public class ArticleEmailWidgetModel
{
    public string ArticleTitle { get; set; } = string.Empty;
    public ImageWidgetModel ArticleTeaserImage { get; set; } = new();
    public string ArticleSummary { get; set; } = string.Empty;
    public string ArticleUrl { get; set; } = string.Empty;

}