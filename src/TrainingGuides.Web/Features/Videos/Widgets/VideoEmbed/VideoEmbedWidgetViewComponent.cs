using System.Web;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TrainingGuides.Web.Features.Videos.Widgets.VideoEmbed;

[assembly: RegisterWidget(
    VideoEmbedWidgetViewComponent.IDENTIFIER,
    typeof(VideoEmbedWidgetViewComponent),
    "Video embed", typeof(VideoEmbedWidgetProperties),
    Description = "Embeds a video in the page.",
    IconClass = "icon-triangle-right")]

namespace TrainingGuides.Web.Features.Videos.Widgets.VideoEmbed;

public class VideoEmbedWidgetViewComponent : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.VideoEmbedWidget";

    public const string HTTPS_SCHEME = "https";
    public const string YOUTUBE_HOST = "www.youtube.com";
    public const string VIMEO_HOST = "player.vimeo.com";
    public const string DAILYMOTION_HOST = "www.dailymotion.com";

    private readonly IStringLocalizer<SharedResources> stringLocalizer;

    public VideoEmbedWidgetViewComponent(IStringLocalizer<SharedResources> stringLocalizer)
    {
        this.stringLocalizer = stringLocalizer;
    }

    public IViewComponentResult Invoke(ComponentViewModel<VideoEmbedWidgetProperties> widgetProperties)
    {
        var markup = GetEmbedMarkup(widgetProperties.Properties);
        return View("~/Features/Videos/Widgets/VideoEmbed/VideoEmbedWidget.cshtml", new VideoEmbedWidgetViewModel { MarkupHtml = markup });
    }

    private HtmlString GetLocalizedHtmlString(string key) => new(stringLocalizer[key]);

    private HtmlString GetEmbedMarkup(VideoEmbedWidgetProperties widgetProperties)
    {
        if (widgetProperties != null && !string.IsNullOrEmpty(widgetProperties.Url))
        {
            return widgetProperties.Service switch
            {
                VideoEmbedWidgetProperties.YOUTUBE => GetYoutubeMarkup(widgetProperties),
                VideoEmbedWidgetProperties.VIMEO => GetVimeoMarkup(widgetProperties),
                VideoEmbedWidgetProperties.DAILYMOTION => GetDailyMotionMarkup(widgetProperties),
                VideoEmbedWidgetProperties.FILE => GetFileMarkup(widgetProperties),
                _ => GetLocalizedHtmlString("Specified video service not found."),
            };
        }
        return GetLocalizedHtmlString("Please make sure the URL property is filled in.");
    }

    private HtmlString GetFileMarkup(VideoEmbedWidgetProperties widgetProperties)
    {
        if (widgetProperties != null && !string.IsNullOrEmpty(widgetProperties.Url))
        {
            string extension = GetFileExtension(widgetProperties.Url);
            if (!string.IsNullOrEmpty(extension))
            {
                string anchor = widgetProperties.PlayFromBeginning ? string.Empty : $"#t={widgetProperties.StartingTime}";

                return new HtmlString(widgetProperties.DynamicSize
                    ? $"<video style=\"width:100%;\" controls><source src=\"{widgetProperties.Url}{anchor}\" type=\"video/{extension}\"></video>"
                    : $"<video width=\"{widgetProperties.Width}\" height=\"{widgetProperties.Height}\" controls><source src=\"{widgetProperties.Url}{anchor}\" type=\"video/{extension}\"></video>"
                );
            }
            return GetLocalizedHtmlString("Unable to parse file extension from the provided Url.");
        }
        return GetLocalizedHtmlString("Please make sure the URL property is filled in.");
    }

    private HtmlString GetVimeoMarkup(VideoEmbedWidgetProperties widgetProperties)
    {
        if (widgetProperties != null && !string.IsNullOrEmpty(widgetProperties.Url))
        {
            string videoId = GetFinalPathComponent(widgetProperties.Url);
            if (!string.IsNullOrEmpty(videoId))
            {
                string anchor = widgetProperties.PlayFromBeginning ? string.Empty : $"t={widgetProperties.StartingTime}s";
                string videoUrl = new UriBuilder()
                {
                    Scheme = HTTPS_SCHEME,
                    Host = VIMEO_HOST,
                    Path = $"video/{videoId}",
                    Fragment = anchor
                }.ToString();

                string scriptUrl = new UriBuilder()
                {
                    Scheme = HTTPS_SCHEME,
                    Host = VIMEO_HOST,
                    Path = "api/player.js",
                }.ToString();

                return new HtmlString(widgetProperties.DynamicSize
                    ? $"<div style=\"padding:56.25% 0 0 0;position:relative;\"><iframe src=\"{videoUrl}\" allow=\"autoplay; fullscreen; picture-in-picture; clipboard-write; encrypted-media\" style=\"position:absolute;top:0;left:0;width:100%;height:100%;\"></iframe></div><script src=\"{scriptUrl}\"></script>"
                    : $"<iframe src=\"{videoUrl}\" width=\"{widgetProperties.Width}\" height=\"{widgetProperties.Height}\" frameborder=\"0\" allow=\"autoplay; fullscreen; picture-in-picture\" allowfullscreen ></iframe >"
                );
            }
            return GetLocalizedHtmlString("Unable to parse Vimeo video ID from the provided Url.");
        }
        return GetLocalizedHtmlString("Please make sure the URL property is filled in.");
    }

    private HtmlString GetDailyMotionMarkup(VideoEmbedWidgetProperties widgetProperties)
    {
        if (widgetProperties != null && !string.IsNullOrEmpty(widgetProperties.Url))
        {
            string videoId = GetFinalPathComponent(widgetProperties.Url);
            if (!string.IsNullOrEmpty(videoId))
            {
                string url = new UriBuilder()
                {
                    Scheme = HTTPS_SCHEME,
                    Host = DAILYMOTION_HOST,
                    Path = $"embed/video/{videoId}"
                }.ToString();

                return new HtmlString(widgetProperties.DynamicSize
                    ? $"<div style=\"position:relative;padding-bottom:56.25%;height:0;overflow:hidden;\"> <iframe style=\"width:100%;height:100%;position:absolute;left:0px;top:0px;overflow:hidden\" frameborder=\"0\" type=\"text/html\" src=\"{url}\" width=\"100%\" height=\"100%\" allowfullscreen title=\"Dailymotion Video Player\" allow=\"autoplay\"></iframe></div>"
                    : $"<iframe src=\"{url}\" width=\"{widgetProperties.Width}\" height=\"{widgetProperties.Height}\" frameborder=\"0\" type=\"text/html\" allowfullscreen title=\"Dailymotion Video Player\"></iframe>"
                );
            }
            return GetLocalizedHtmlString("Unable to parse Dailymotion video ID from the provided Url.");
        }
        return GetLocalizedHtmlString("Please make sure the URL property is filled in.");
    }

    private HtmlString GetYoutubeMarkup(VideoEmbedWidgetProperties widgetProperties)
    {
        if (widgetProperties != null && !string.IsNullOrEmpty(widgetProperties.Url))
        {
            string videoId = GetYoutubeId(widgetProperties.Url);
            if (!string.IsNullOrEmpty(videoId))
            {
                string query = widgetProperties.PlayFromBeginning ? string.Empty : $"start={widgetProperties.StartingTime}";

                string url = new UriBuilder()
                {
                    Scheme = HTTPS_SCHEME,
                    Host = YOUTUBE_HOST,
                    Path = $"embed/{videoId}",
                    Query = query
                }.ToString();

                return new HtmlString($"<iframe width=\"{widgetProperties.Width}\" height=\"{widgetProperties.Height}\" src=\"{url}\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer;autoplay;clipboard-write;encrypted-media;gyroscope;picture-in-picture;web-share\" allowfullscreen></iframe>");
            }
            return GetLocalizedHtmlString("Unable to parse Youtube video ID from the provided Url.");
        }
        return GetLocalizedHtmlString("Please make sure the URL property is filled in.");
    }

    private string GetYoutubeId(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            string? queryId = GetIdFromQuery(url, "v");
            return string.IsNullOrEmpty(queryId) ? GetFinalPathComponent(url) : queryId;
        }
        return string.Empty;
    }

    private string? GetIdFromQuery(string url, string paramName)
    {
        if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(paramName))
        {
            var uri = new Uri(url);
            if (!string.IsNullOrEmpty(uri.Query))
            {
                var query = HttpUtility.ParseQueryString(uri.Query);
                if (query != null)
                {
                    return query.Get(paramName);
                }
            }
        }
        return string.Empty;
    }

    private string GetFinalPathComponent(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            string baseUrl = url.Split('?')[0];
            string[] urlComponents = baseUrl.Split('/');

            if (urlComponents.Length > 3)
            {
                return urlComponents[^1];
            }
        }
        return string.Empty;
    }

    private string GetFileExtension(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            string finalComponent = GetFinalPathComponent(url);
            string[] parts = finalComponent.Split('.');
            if (parts.Length > 1)
            {
                return parts[^1];
            }
        }
        return string.Empty;
    }
}
