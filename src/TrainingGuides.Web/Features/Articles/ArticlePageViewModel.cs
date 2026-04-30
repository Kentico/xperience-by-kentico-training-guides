using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Articles;

public class ArticlePageViewModel
{
    public string Title { get; set; } = string.Empty;
    public HtmlString SummaryHtml { get; set; } = HtmlString.Empty;
    public HtmlString TextHtml { get; set; } = HtmlString.Empty;
    public AssetViewModel? TeaserImage { get; set; } = null;
    public DateTime CreatedOn { get; set; }
    public List<ArticlePageViewModel> RelatedNews { get; set; } = [];
    public string Url { get; set; } = string.Empty;
    public bool Restricted { get; set; } = false;
    public bool RequiresSignIn { get; set; } = false;
    public string CTAText { get; set; } = string.Empty;
}
