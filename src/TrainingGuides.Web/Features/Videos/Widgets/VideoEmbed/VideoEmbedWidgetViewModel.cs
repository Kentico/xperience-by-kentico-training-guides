using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.Videos.Widgets.VideoEmbed;

public class VideoEmbedWidgetViewModel
{
    public HtmlString Markup { get; set; } = HtmlString.Empty;
}