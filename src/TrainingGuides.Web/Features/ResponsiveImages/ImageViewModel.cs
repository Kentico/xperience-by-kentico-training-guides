using CMS.ContentEngine;

namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// Pairs an image asset with its alternative text for rendering in views.
/// </summary>
/// <param name="Asset">The image asset, including its variant URLs and dimension metadata.</param>
/// <param name="AltText">Alternative text for the image, or an empty string for decorative images.</param>
public record ImageViewModel(ContentItemAsset Asset, string AltText)
{
    /// <summary>
    /// Creates an <see cref="ImageViewModel"/> from an <see cref="Asset"/> content item.
    /// </summary>
    /// <param name="asset">The source Asset content item, or <c>null</c>.</param>
    /// <returns>
    /// A populated <see cref="ImageViewModel"/>, or <c>null</c> if the item or its file is missing.
    /// Null alternative text is normalized to an empty string.
    /// </returns>
    public static ImageViewModel? GetViewModel(Asset? asset) => asset?.AssetFile is null
        ? null
        : new(asset.AssetFile, asset.AssetAltText ?? string.Empty);
}
