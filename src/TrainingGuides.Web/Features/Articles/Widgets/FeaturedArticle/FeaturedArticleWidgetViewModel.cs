using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedArticle;

public class FeaturedArticleWidgetViewModel : IWidgetViewModel
{
    public ArticlePageViewModel? Article { get; set; }

    public bool IsMisconfigured => Article == null;
}
