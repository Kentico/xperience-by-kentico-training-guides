using System.Web;
using Kentico.PageBuilder.Web.Mvc;
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

    private readonly IStringLocalizer<SharedResources> stringLocalizer;

    public VideoEmbedWidgetViewComponent(IStringLocalizer<SharedResources> stringLocalizer)
    {
        this.stringLocalizer = stringLocalizer;
    }

    public IViewComponentResult Invoke(ComponentViewModel<VideoEmbedWidgetProperties> widgetProperties)
    {
        string markup = GetEmbedMarkup(widgetProperties.Properties);
        return View("~/Features/Videos/Widgets/VideoEmbed/VideoEmbedWidget.cshtml", new VideoEmbedWidgetViewModel { Markup = markup });
    }

    private string GetEmbedMarkup(VideoEmbedWidgetProperties widgetProperties)
    {
        if (widgetProperties != null && !string.IsNullOrEmpty(widgetProperties.Url))
        {
            return widgetProperties.Service switch
            {
                VideoEmbedWidgetProperties.YOUTUBE => GetYoutubeMarkup(widgetProperties),
                VideoEmbedWidgetProperties.VIMEO => GetVimeoMarkup(widgetProperties),
                VideoEmbedWidgetProperties.DAILYMOTION => GetDailyMotionMarkup(widgetProperties),
                VideoEmbedWidgetProperties.FILE => GetFileMarkup(widgetProperties),
                _ => stringLocalizer["Specified video service not found."],
            };
        }
        return stringLocalizer["Please make sure the URL property is filled in."];
    }

    private string GetFileMarkup(VideoEmbedWidgetProperties widgetProperties)
    {
        if (widgetProperties != null && !string.IsNullOrEmpty(widgetProperties.Url))
        {
            string extension = GetFileExtension(widgetProperties.Url);
            if (!string.IsNullOrEmpty(extension))
            {
                string anchor = widgetProperties.PlayFromBeginning ? string.Empty : $"#t={widgetProperties.StartingTime}";
                if (widgetProperties.DynamicSize)
                {
                    return $"<video style=\"width:100%;\" controls><source src=\"{widgetProperties.Url}{anchor}\" type=\"video/{extension}\"></video>";
                }
                else
                {
                    return $"<video width=\"{widgetProperties.Width}\" height=\"{widgetProperties.Height}\" controls><source src=\"{widgetProperties.Url}{anchor}\" type=\"video/{extension}\"></video>";
                }
            }
            return stringLocalizer["Unable to parse file extension from the provided Url."];
        }
        return stringLocalizer["Please make sure the URL property is filled in."];
    }

    private string GetVimeoMarkup(VideoEmbedWidgetProperties widgetProperties)
    {
        if (widgetProperties != null && !string.IsNullOrEmpty(widgetProperties.Url))
        {
            string videoId = GetFinalPathComponent(widgetProperties.Url);
            if (!string.IsNullOrEmpty(videoId))
            {
                string anchor = widgetProperties.PlayFromBeginning ? string.Empty : $"#t={widgetProperties.StartingTime}s";
                if (widgetProperties.DynamicSize)
                {

                    return "<div style=\"padding: 56.25 % 0 0 0; position: relative;\"><iframe src=\"https://player.vimeo.com/video/{videoId}{anchor}\" style=\"position:absolute;top:0;left:0;width:100%;height:100%;\" frameborder=\"0\" allow=\"autoplay; fullscreen; picture-in-picture\" allowfullscreen></iframe></div><script src=\"https://player.vimeo.com/api/player.js\"></script>";
                }
                else
                {
                    return $"<iframe src=\"https://player.vimeo.com/video/{videoId}{anchor}\" width=\"{widgetProperties.Width}\" height=\"{widgetProperties.Height}\" frameborder=\"0\" allow=\"autoplay; fullscreen; picture-in-picture\" allowfullscreen ></iframe >";
                }
            }
            return stringLocalizer["Unable to parse Vimeo video ID from the provided Url."];
        }
        return stringLocalizer["Please make sure the URL property is filled in."];
    }

    private string GetDailyMotionMarkup(VideoEmbedWidgetProperties widgetProperties)
    {
        if (widgetProperties != null && !string.IsNullOrEmpty(widgetProperties.Url))
        {
            string videoId = GetFinalPathComponent(widgetProperties.Url);
            if (!string.IsNullOrEmpty(videoId))
            {
                if (widgetProperties.DynamicSize)
                {
                    return $"<div style=\"position:relative;padding-bottom:56.25%;height:0;overflow:hidden;\"> <iframe style=\"width:100%;height:100%;position:absolute;left:0px;top:0px;overflow:hidden\" frameborder=\"0\" type=\"text/html\" src=\"https://www.dailymotion.com/embed/video/{videoId}\" width=\"100%\" height=\"100%\" allowfullscreen title=\"Dailymotion Video Player\" allow=\"autoplay\"></iframe></div>";
                }
                else
                {
                    return $"<iframe src=\"https://www.dailymotion.com/embed/video/{videoId}\" width=\"{widgetProperties.Width}\" height=\"{widgetProperties.Height}\" frameborder=\"0\" type=\"text/html\" allowfullscreen title=\"Dailymotion Video Player\"></iframe>";
                }
            }
            return stringLocalizer["Unable to parse Dailymotion video ID from the provided Url."];
        }
        return stringLocalizer["Please make sure the URL property is filled in."];
    }

    private string GetYoutubeMarkup(VideoEmbedWidgetProperties widgetProperties)
    {
        if (widgetProperties != null && !string.IsNullOrEmpty(widgetProperties.Url))
        {
            string videoId = GetYoutubeId(widgetProperties.Url);
            if (!string.IsNullOrEmpty(videoId))
            {
                string query = widgetProperties.PlayFromBeginning ? string.Empty : $"?start={widgetProperties.StartingTime}";
                return $"<iframe width=\"{widgetProperties.Width}\" height=\"{widgetProperties.Height}\" src=\"https://www.youtube.com/embed/{videoId}{query}\" title=\"YouTube video player\" frameborder=\"0\" allow=\"accelerometer;autoplay;clipboard-write;encrypted-media;gyroscope;picture-in-picture;web-share\" allowfullscreen></iframe>";
            }
            return stringLocalizer["Unable to parse Youtube video ID from the provided Url."];
        }
        return stringLocalizer["Please make sure the URL property is filled in."];
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
