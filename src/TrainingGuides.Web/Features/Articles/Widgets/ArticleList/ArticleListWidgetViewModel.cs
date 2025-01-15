using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetViewModel : WidgetViewModel
{
    public List<ArticlePageViewModel> Articles { get; set; } = [];
    public string CtaText { get; set; } = string.Empty;

    public override bool IsMisconfigured => Articles == null || Articles.Count == 0;
}