using CMS.ContentEngine;

namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// An image asset together with the alternative text that describes it.
/// </summary>
/// <param name="Asset">The image file, including its variant URLs and metadata.</param>
/// <param name="AltText">Text that describes the image; empty for a decorative image.</param>
public record ImageViewModel(ContentItemAsset Asset, string AltText)
{
    /// <summary>
    /// Creates an image view model from an Asset content item.
    /// </summary>
    /// <param name="asset">The Asset content item, or <c>null</c>.</param>
    /// <returns>The view model, or <c>null</c> when there is no asset or file.</returns>
    public static ImageViewModel? GetViewModel(Asset? asset) => asset?.AssetFile is null
        ? null
        : new(asset.AssetFile, asset.AssetAltText ?? string.Empty);
}
