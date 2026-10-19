namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// View model for the image variants showcase component.
/// </summary>
public class ImageVariantsShowcaseViewModel
{
    /// <summary>
    /// The article title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The teaser image and alt text, or <c>null</c> when no teaser asset is available.
    /// </summary>
    public ImageViewModel? Image { get; set; }
}
