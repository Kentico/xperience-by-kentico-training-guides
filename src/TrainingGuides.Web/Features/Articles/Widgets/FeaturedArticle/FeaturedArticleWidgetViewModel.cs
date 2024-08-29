using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedArticle;

public class FeaturedArticleWidgetViewModel : WidgetViewModel
{
    public ArticlePageViewModel Article { get; set; } = new ArticlePageViewModel();

    public override bool IsMisconfigured => string.IsNullOrWhiteSpace(Article.Title)
        || string.IsNullOrWhiteSpace(Article.Summary.ToString())
        || Article.TeaserImage == null
        || string.IsNullOrWhiteSpace(Article.Url);
}
