using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.Videos.Widgets.VideoEmbed;

public class VideoEmbedWidgetViewModel
{
    public HtmlString MarkupHtml { get; set; } = HtmlString.Empty;
}
