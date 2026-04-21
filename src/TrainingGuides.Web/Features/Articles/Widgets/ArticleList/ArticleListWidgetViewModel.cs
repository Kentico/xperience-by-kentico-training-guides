using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetViewModel : IWidgetViewModel
{
    public List<ArticlePageViewModel> Articles { get; set; } = [];
    public bool IsMisconfigured => Articles == null || Articles.Count == 0;

}
