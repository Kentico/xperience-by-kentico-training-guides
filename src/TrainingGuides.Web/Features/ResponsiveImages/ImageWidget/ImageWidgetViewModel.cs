using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.ResponsiveImages.ImageWidget;

/// <summary>
/// The image and image variant shown by the "Image" widget.
/// </summary>
public class ImageWidgetViewModel : IWidgetViewModel
{
    /// <summary>
    /// The image, or <c>null</c> when no asset is picked, the asset item isn't found
    /// or it has no file.
    /// </summary>
    public ImageViewModel? Image { get; set; }

    /// <summary>
    /// Code name of the picked image variant, or <c>null</c> for the responsive image.
    /// </summary>
    public string? ImageVariant { get; set; }

    /// <inheritdoc/>
    public bool IsMisconfigured => Image is null;
}
