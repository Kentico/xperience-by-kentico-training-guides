using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Videos.Widgets.VideoEmbed;

public class VideoEmbedWidgetProperties : IWidgetProperties
{
    public const string YOUTUBE = "youtube";
    public const string YOUTUBE_DESC = "YouTube";
    public const string VIMEO = "vimeo";
    public const string VIMEO_DESC = "Vimeo";
    public const string DAILYMOTION = "dailymotion";
    public const string DAILYMOTION_DESC = "Dailymotion";
    public const string FILE = "file";
    public const string FILE_DESC = "Video file URL";

    public const string SERVICE_OPTIONS = YOUTUBE + ";" + YOUTUBE_DESC + "\r\n" +
                                    VIMEO + ";" + VIMEO_DESC + "\r\n" +
                                    DAILYMOTION + ";" + DAILYMOTION_DESC + "\r\n" +
                                    FILE + ";" + FILE_DESC + "\r\n";

    /// <summary>
    /// Defines the video platform from which the embedded video originates.
    /// </summary>
    [RadioGroupComponent(Label = "Video service", Inline = true, Order = 1, Options = SERVICE_OPTIONS)]
    public string Service { get; set; } = YOUTUBE;


    /// <summary>
    /// Defines the URL of the embedded video.
    /// </summary>
    [TextInputComponent(Label = "Url", Order = 2)]
    public string Url { get; set; }


    /// <summary>
    /// Determines whether the video should be sized dynamically or with explicit dimensions.
    /// </summary>
    [CheckBoxComponent(Label = "Size dynamically", Order = 3)]
    [VisibleIfNotEqualTo(nameof(Service), YOUTUBE)]
    public bool DynamicSize { get; set; } = true;


    /// <summary>
    /// Determines the width of the embed.
    /// </summary>
    [NumberInputComponent(Label = "Width (px)", Order = 4)]
    [FormComponentConfiguration(typeof(VideoEmbedWidgetConfigurator), [nameof(Service), nameof(DynamicSize)])]
    public int Width { get; set; } = 560;


    /// <summary>
    /// Detemines the height of the embed.
    /// </summary>
    [NumberInputComponent(Label = "Height (px)", Order = 5)]
    [FormComponentConfiguration(typeof(VideoEmbedWidgetConfigurator), [nameof(Service), nameof(DynamicSize)])]
    public int Height { get; set; } = 315;


    /// <summary>
    /// Determines whether the video will start at the beginning, or at a specified timestamp.
    /// </summary>        
    [CheckBoxComponent(Label = "Play from beginning", Order = 6)]
    [VisibleIfNotEqualTo(nameof(Service), DAILYMOTION)]
    public bool PlayFromBeginning { get; set; } = true;


    /// <summary>
    /// Defines the time to start the player at.
    /// </summary>
    [NumberInputComponent(Label = "Starting time (seconds)", Order = 7)]
    [VisibleIfFalse(nameof(PlayFromBeginning))]
    [VisibleIfNotEqualTo(nameof(Service), DAILYMOTION)]
    public int StartingTime { get; set; } = 0;
}

