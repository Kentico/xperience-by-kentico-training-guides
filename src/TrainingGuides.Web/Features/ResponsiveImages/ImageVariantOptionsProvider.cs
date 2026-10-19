using CMS.ContentEngine;
using CMS.DataEngine;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// Populates a drop-down with all configured image variant definitions,
/// preceded by an "Automatic (responsive)" option with an empty value.
/// </summary>
/// <param name="standardMediaDimensionsInfoProvider">
/// Provider used to query image variant definitions.
/// </param>
public class ImageVariantOptionsProvider(
    IInfoProvider<StandardMediaDimensionsInfo> standardMediaDimensionsInfoProvider)
    : IDropDownOptionsProvider
{
    /// <summary>
    /// Label for the default option, whose empty value selects the responsive width series.
    /// </summary>
    public const string AUTOMATIC_OPTION_TEXT = "Automatic (responsive)";

    private const string SMART_CROP_LABEL = "Smart crop";
    private const string SCALED_LABEL = "Scaled";

    /// <inheritdoc/>
    public async Task<IEnumerable<DropDownOptionItem>> GetOptionItems()
    {
        var definitions = await standardMediaDimensionsInfoProvider.Get()
            .GetEnumerableTypedResultAsync();

        return BuildOptionItems(definitions.Select(definition => new VariantDefinition(
            definition.StandardMediaDimensionsName,
            definition.StandardMediaDimensionsDisplayName,
            definition.StandardMediaDimensionsWidth,
            definition.StandardMediaDimensionsHeight,
            definition.StandardMediaDimensionsTransformationType)));
    }

    /// <summary>
    /// Orders variant definitions (Smart crop before Scaled, then by width and height)
    /// and formats them as drop-down options after the default automatic option.
    /// </summary>
    /// <param name="definitions">The image variant definitions to format.</param>
    /// <returns>Drop-down options with variant code names as values.</returns>
    internal static IEnumerable<DropDownOptionItem> BuildOptionItems(
        IEnumerable<VariantDefinition> definitions)
    {
        var variantOptions = definitions
            .OrderBy(definition =>
                definition.TransformationType == MediaTransformationType.SmartCrop ? 0 : 1)
            .ThenBy(definition => definition.Width)
            .ThenBy(definition => definition.Height)
            .Select(definition => new DropDownOptionItem
            {
                Value = definition.CodeName,
                Text = $"{definition.DisplayName} ({GetCaption(definition)})"
            });

        return
        [
            new DropDownOptionItem { Value = string.Empty, Text = AUTOMATIC_OPTION_TEXT },
            .. variantOptions
        ];
    }

    /// <summary>
    /// Formats a definition's pixel dimensions and transformation type (for example,
    /// <c>800 × 600 px · Smart crop</c> or <c>480 px · Scaled</c>), omitting a zero dimension.
    /// </summary>
    /// <param name="definition">The image variant definition.</param>
    /// <returns>The formatted caption string.</returns>
    private static string GetCaption(VariantDefinition definition)
    {
        string size = (definition.Width > 0, definition.Height > 0) switch
        {
            (true, true) => $"{definition.Width} × {definition.Height} px",
            (true, false) => $"{definition.Width} px",
            _ => $"{definition.Height} px"
        };

        string transformation =
            definition.TransformationType == MediaTransformationType.SmartCrop
                ? SMART_CROP_LABEL
                : SCALED_LABEL;

        return $"{size} · {transformation}";
    }

    /// <summary>
    /// Projection of <see cref="StandardMediaDimensionsInfo"/> used to sort and format options.
    /// </summary>
    /// <param name="CodeName">The definition's code name, used as the option value.</param>
    /// <param name="DisplayName">The definition's display name.</param>
    /// <param name="Width">Width in pixels, or 0 when calculated from the aspect ratio.</param>
    /// <param name="Height">Height in pixels, or 0 when calculated from the aspect ratio.</param>
    /// <param name="TransformationType">The variant transformation type.</param>
    internal record VariantDefinition(string CodeName, string DisplayName, int Width,
        int Height, MediaTransformationType TransformationType);
}
