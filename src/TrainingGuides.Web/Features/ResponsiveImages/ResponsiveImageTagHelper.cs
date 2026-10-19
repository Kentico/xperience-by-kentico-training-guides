using System.Diagnostics.CodeAnalysis;
using CMS.ContentEngine;
using CMS.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// Renders an <c>img</c> element for a <see cref="ContentItemAsset"/> using the responsive
/// <see cref="ImageVariants.WidthSeries"/> variants or a single <see cref="Variant"/> override.
/// Suppresses output when the asset is <c>null</c> or is not a supported image format.
/// </summary>
/// <param name="urlHelperFactory">Factory used to resolve app-relative asset URLs.</param>
[HtmlTargetElement("tg-responsive-image", TagStructure = TagStructure.WithoutEndTag)]
public class ResponsiveImageTagHelper(IUrlHelperFactory urlHelperFactory) : TagHelper
{
    private const string IMG_TAG = "img";

    /// <summary>
    /// View context injected by Razor for resolving application-relative URLs.
    /// </summary>
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    /// <summary>
    /// The image asset to render.
    /// </summary>
    public ContentItemAsset? Asset { get; set; }

    /// <summary>
    /// Alternative text for the image; defaults to an empty string for decorative images.
    /// </summary>
    public string Alt { get; set; } = string.Empty;

    /// <summary>
    /// Optional variant code name to render as a single fixed image. Falls back to the
    /// responsive <c>srcset</c> when <c>null</c> or when the asset lacks the variant.
    /// </summary>
    public string? Variant { get; set; }

    /// <summary>
    /// The HTML <c>sizes</c> attribute describing the rendered image width per breakpoint.
    /// </summary>
    public string Sizes { get; set; } = "100vw";

    /// <summary>
    /// The HTML <c>loading</c> attribute (<c>lazy</c> by default, or <c>eager</c> above the fold).
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

        // Page Builder and preview only expand "~/" at the start of an attribute value,
        // so resolve every candidate URL before joining srcset.
        var urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);

        if (TryGetVariant(Asset, Variant, out string? variantUrl, out var variantMetadata))
        {
            output.Attributes.SetAttribute("src", urlHelper.Content(variantUrl));
            SetDimensions(output, variantMetadata.Width, variantMetadata.Height);
            return;
        }

        var candidates = GetWidthCandidates(Asset);

        if (candidates.Count == 0)
        {
            output.Attributes.SetAttribute("src", urlHelper.Content(Asset.Url));
        }
        else
        {
            string srcset = string.Join(", ", candidates.Select(
                candidate => $"{urlHelper.Content(candidate.Url)} {candidate.Width}w"));

            output.Attributes.SetAttribute("src", urlHelper.Content(candidates[^1].Url));
            output.Attributes.SetAttribute("srcset", srcset);
            output.Attributes.SetAttribute("sizes", Sizes);
        }

        // Scaled variants preserve the original aspect ratio.
        SetDimensions(output, Asset.Metadata.Width, Asset.Metadata.Height);
    }

    /// <summary>
    /// Collects width-series variants that do not exceed the original image width, ordered
    /// from narrowest to widest, and appends the original image when wider variants were skipped.
    /// </summary>
    /// <param name="asset">The source image asset.</param>
    /// <returns>Candidate URLs and pixel widths, or an empty list if none are available.</returns>
    private static List<(string Url, int Width)> GetWidthCandidates(ContentItemAsset asset)
    {
        int originalWidth = asset.Metadata.Width ?? int.MaxValue;
        var candidates = new List<(string Url, int Width)>();
        bool seriesExceedsOriginal = false;

        foreach (string variant in ImageVariants.WidthSeries)
        {
            if (!TryGetVariant(asset, variant, out string? url, out var metadata) || metadata.Width is not int width)
            {
                continue;
            }

            if (width > originalWidth)
            {
                seriesExceedsOriginal = true;
                continue;
            }

            candidates.Add((url, width));
        }

        // Append the original only when its width is not already covered by the last variant,
        // since HTML srcset forbids duplicate width descriptors.
        if (seriesExceedsOriginal && (candidates.Count == 0 || candidates[^1].Width < originalWidth))
        {
            candidates.Add((asset.Url, originalWidth));
        }

        return candidates;
    }

    /// <summary>
    /// Looks up a variant's URL and dimension metadata by its exact, case-sensitive code name.
    /// </summary>
    /// <param name="asset">The source image asset.</param>
    /// <param name="codeName">The variant code name to look up.</param>
    /// <param name="url">When this method returns, contains the variant URL if found.</param>
    /// <param name="metadata">When this method returns, contains the variant metadata if found.</param>
    /// <returns>
    /// <c>true</c> if the asset contains both a URL and metadata for <paramref name="codeName"/>.
    /// </returns>
    private static bool TryGetVariant(
        ContentItemAsset asset,
        string? codeName,
        [NotNullWhen(true)] out string? url,
        [NotNullWhen(true)] out ContentItemAssetVariantMetadata? metadata)
    {
        url = null;
        metadata = null;

        // Both dictionaries are null when an asset has no variants, such as an SVG.
        return !string.IsNullOrEmpty(codeName)
            && asset.VariantUrls?.TryGetValue(codeName, out url) == true
            && asset.Metadata.Variants?.TryGetValue(codeName, out metadata) == true;
    }

    /// <summary>
    /// Writes <c>width</c> and <c>height</c> attributes when both dimensions are positive.
    /// </summary>
    /// <param name="output">The tag helper output to modify.</param>
    /// <param name="width">Width in pixels, or <c>null</c>.</param>
    /// <param name="height">Height in pixels, or <c>null</c>.</param>
    private static void SetDimensions(TagHelperOutput output, int? width, int? height)
    {
        if (width > 0 && height > 0)
        {
            output.Attributes.SetAttribute("width", width);
            output.Attributes.SetAttribute("height", height);
        }
    }
}
