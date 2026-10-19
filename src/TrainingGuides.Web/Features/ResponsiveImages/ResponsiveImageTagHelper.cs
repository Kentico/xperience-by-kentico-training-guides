using System.Diagnostics.CodeAnalysis;
using CMS.ContentEngine;
using CMS.Helpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// Renders an <c>img</c> element for an image asset. Without a fixed variant, the image gets
/// a <c>srcset</c> of the <see cref="ImageVariants.WidthLadder"/> variants with <c>w</c> descriptors,
/// so the browser downloads the smallest file that fills the slot described by <see cref="Sizes"/>.
/// Renders nothing when the asset is missing or isn't an image.
/// </summary>
[HtmlTargetElement("tg-responsive-image", TagStructure = TagStructure.WithoutEndTag)]
public class ResponsiveImageTagHelper : TagHelper
{
    private const string IMG_TAG = "img";

    /// <summary>
    /// The image asset to render.
    /// </summary>
    public ContentItemAsset? Asset { get; set; }

    /// <summary>
    /// Alternative text, always rendered; an empty value marks the image as decorative.
    /// </summary>
    public string Alt { get; set; } = string.Empty;

    /// <summary>
    /// Code name of a variant that replaces the responsive <c>srcset</c> with that one variant and its dimensions.
    /// When the asset has no such variant, the responsive <c>srcset</c> is rendered instead.
    /// </summary>
    public string? Variant { get; set; }

    /// <summary>
    /// The rendered width of the image slot per breakpoint, in the HTML <c>sizes</c> syntax.
    /// </summary>
    public string Sizes { get; set; } = "100vw";

    /// <summary>
    /// The HTML <c>loading</c> value: <c>lazy</c> below the fold, <c>eager</c> for the main image of a page.
    /// </summary>
    public string Loading { get; set; } = "lazy";

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Asset is null || !ImageHelper.IsImage(Asset.Metadata.Extension))
        {
            output.SuppressOutput();
            return;
        }

        output.TagName = IMG_TAG;
        output.TagMode = TagMode.SelfClosing;
        output.Attributes.SetAttribute("alt", Alt);
        output.Attributes.SetAttribute("loading", Loading);

        if (TryGetVariant(Asset, Variant, out string? variantUrl, out var variantMetadata))
        {
            output.Attributes.SetAttribute("src", variantUrl);
            SetDimensions(output, variantMetadata.Width, variantMetadata.Height);
            return;
        }

        var candidates = GetWidthCandidates(Asset);

        if (candidates.Count == 0)
        {
            output.Attributes.SetAttribute("src", Asset.Url);
        }
        else
        {
            output.Attributes.SetAttribute("src", candidates[^1].Url);
            output.Attributes.SetAttribute("srcset", string.Join(", ", candidates.Select(candidate => $"{candidate.Url} {candidate.Width}w")));
            output.Attributes.SetAttribute("sizes", Sizes);
        }

        // Scaled variants keep the original's aspect ratio, so its dimensions reserve the right space
        SetDimensions(output, Asset.Metadata.Width, Asset.Metadata.Height);
    }

    /// <summary>
    /// Lists the width ladder variants of an asset that are no wider than the original, narrowest first.
    /// The original closes the list when the ladder goes past its width and no rung has that width already,
    /// so the largest candidate is never upscaled and no width descriptor appears twice in the list.
    /// </summary>
    /// <param name="asset">The image asset.</param>
    /// <returns>The URL and actual width of each candidate; empty when the asset has none of the ladder variants.</returns>
    private static List<(string Url, int Width)> GetWidthCandidates(ContentItemAsset asset)
    {
        int originalWidth = asset.Metadata.Width ?? int.MaxValue;
        var candidates = new List<(string Url, int Width)>();
        bool ladderExceedsOriginal = false;

        foreach (string variant in ImageVariants.WidthLadder)
        {
            if (!TryGetVariant(asset, variant, out string? url, out var metadata) || metadata.Width is not int width)
            {
                continue;
            }

            if (width > originalWidth)
            {
                ladderExceedsOriginal = true;
                continue;
            }

            candidates.Add((url, width));
        }

        // A rung stored at exactly the original's width already closes the list, and srcset forbids repeating a descriptor
        if (ladderExceedsOriginal && (candidates.Count == 0 || candidates[^1].Width < originalWidth))
        {
            candidates.Add((asset.Url, originalWidth));
        }

        return candidates;
    }

    /// <summary>
    /// Looks up a variant's URL and metadata by its exact, case-sensitive code name.
    /// </summary>
    /// <param name="asset">The image asset.</param>
    /// <param name="codeName">The variant code name, or <c>null</c>.</param>
    /// <param name="url">The variant URL when found.</param>
    /// <param name="metadata">The variant's stored metadata, including its actual size, when found.</param>
    /// <returns><c>true</c> when the asset has the variant; <c>false</c> for no code name, an asset without variants or an unknown code name.</returns>
    private static bool TryGetVariant(
        ContentItemAsset asset,
        string? codeName,
        [NotNullWhen(true)] out string? url,
        [NotNullWhen(true)] out ContentItemAssetVariantMetadata? metadata)
    {
        url = null;
        metadata = null;

        // Both dictionaries are null for an asset without variants, such as an SVG
        return !string.IsNullOrEmpty(codeName)
            && asset.VariantUrls?.TryGetValue(codeName, out url) == true
            && asset.Metadata.Variants?.TryGetValue(codeName, out metadata) == true;
    }

    /// <summary>
    /// Renders <c>width</c> and <c>height</c> when both are known.
    /// </summary>
    /// <param name="output">The tag helper output.</param>
    /// <param name="width">The width in pixels, or <c>null</c>.</param>
    /// <param name="height">The height in pixels, or <c>null</c>.</param>
    private static void SetDimensions(TagHelperOutput output, int? width, int? height)
    {
        if (width > 0 && height > 0)
        {
            output.Attributes.SetAttribute("width", width);
            output.Attributes.SetAttribute("height", height);
        }
    }
}
