using System.Diagnostics.CodeAnalysis;
using CMS.Helpers;
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

    /// <summary>
    /// <c>true</c> when there is no image or the picked file isn't an image.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Image))]
    public bool IsMisconfigured =>
        Image is null || !ImageHelper.IsImage(Image.Asset.Metadata.Extension);
}
