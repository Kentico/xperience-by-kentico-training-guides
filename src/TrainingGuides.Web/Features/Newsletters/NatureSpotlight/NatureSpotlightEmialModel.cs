using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;
using Microsoft.AspNetCore.Components;
using TrainingGuides.Web.Features.Articles.EmailWidgets;

namespace TrainingGuides.Web.Features.Newsletters.NatureSpotlight;

public class NatureSpotlightEmailModel
{
    public string Subject { get; set; } = string.Empty;

    public string PreviewText { get; set; } = string.Empty;

    public IEnumerable<string> Countries { get; set; } = Enumerable.Empty<string>();

    public string Topic { get; set; } = string.Empty;

    public MarkupString Text { get; set; } = new MarkupString();

    public IEnumerable<ImageWidgetModel> Images { get; set; } = Enumerable.Empty<ImageWidgetModel>();

    public IEnumerable<ArticleEmailWidgetModel> RelatedArticles { get; set; } = Enumerable.Empty<ArticleEmailWidgetModel>();
}