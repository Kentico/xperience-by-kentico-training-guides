using System.Diagnostics.CodeAnalysis;
using CMS.Helpers;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.ResponsiveImages.ImageWidget;

/// <summary>
/// View model for the "Image" widget.
/// </summary>
public class ImageWidgetViewModel : IWidgetViewModel
{
    /// <summary>
    /// The selected image and its alt text, or <c>null</c> when no valid asset is available.
    /// </summary>
    public ImageViewModel? Image { get; set; }

    /// <summary>
    /// Code name of the selected image variant, or <c>null</c> for the responsive width series.
    /// </summary>
    public string? ImageVariant { get; set; }

    /// <summary>
    /// Indicates whether the widget lacks a valid image file to render.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Image))]
    public bool IsMisconfigured =>
        Image is null || !ImageHelper.IsImage(Image.Asset.Metadata.Extension);
}
