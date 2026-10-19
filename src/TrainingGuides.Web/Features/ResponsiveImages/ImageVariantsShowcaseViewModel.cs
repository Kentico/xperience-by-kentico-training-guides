namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// The current article's title and teaser image, shown by each block of the image variants showcase.
/// </summary>
public class ImageVariantsShowcaseViewModel
{
    /// <summary>
    /// The article title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The teaser image, or <c>null</c> when the page has no article or the article has no teaser.
    /// </summary>
    public ImageViewModel? Image { get; set; }
}
