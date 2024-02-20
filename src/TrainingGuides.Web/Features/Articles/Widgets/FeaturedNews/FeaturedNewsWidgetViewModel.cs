using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles.Widgets.FeaturedNews;

public class FeaturedNewsWidgetViewModel : WidgetViewModel
{
    public ArticlePageViewModel? Article { get; set; }

    public override bool IsMisconfigured => Article == null;
}
