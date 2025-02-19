using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles.Widgets.ArticleList;

public class ArticleListWidgetViewModel : IWidgetViewModel
{
    public List<ArticlePageViewModel> Articles { get; set; } = [];
    public string CtaText { get; set; } = string.Empty;
    public string SignInText { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; } = false;
    public bool IsMisconfigured => Articles == null || Articles.Count == 0;

}