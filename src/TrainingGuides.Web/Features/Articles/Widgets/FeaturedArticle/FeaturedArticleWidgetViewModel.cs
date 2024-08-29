using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedArticle;

public class FeaturedArticleWidgetViewModel : WidgetViewModel
{
    public ArticlePageViewModel? Article { get; set; }

    public override bool IsMisconfigured => Article == null;
}
