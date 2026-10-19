using CMS.ContentEngine;
using CMS.DataEngine;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.ResponsiveImages;

/// <summary>
/// Lists the image variant definitions as drop-down options, after an
/// "Automatic (responsive)" option with an empty value. Each option's value is
/// the variant code name, and its text is the display name with the size and
/// transformation, for example <c>Card 4:3 (800 × 600 px · Smart crop)</c>.
/// </summary>
/// <param name="standardMediaDimensionsInfoProvider">
/// Retrieves the image variant definitions.
/// </param>
public class ImageVariantOptionsProvider(
    IInfoProvider<StandardMediaDimensionsInfo> standardMediaDimensionsInfoProvider)
    : IDropDownOptionsProvider
{
    /// <summary>
    /// Text of the first option, whose empty value means the responsive image.
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
    /// Builds the drop-down options: "Automatic (responsive)" first, then the
    /// Smart crop variants, then the Scaled variants, each group ordered by width
    /// and then by height.
    /// </summary>
    /// <param name="definitions">The image variant definitions.</param>
    /// <returns>The options, with the variant code name as each value.</returns>
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
    /// Describes a definition's size and transformation, for example
    /// <c>800 × 600 px · Smart crop</c> or <c>480 px · Scaled</c>. A side set to 0
    /// is left out, because the Scale transformation calculates that side from
    /// the original's aspect ratio.
    /// </summary>
    /// <param name="definition">The image variant definition.</param>
    /// <returns>The caption.</returns>
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
    /// The fields of an image variant definition used in the option text and order.
    /// </summary>
    /// <param name="CodeName">The definition's code name, used as the option value.</param>
    /// <param name="DisplayName">The definition's display name.</param>
    /// <param name="Width">
    /// The width in pixels; 0 for a Scale definition with an automatic width.
    /// </param>
    /// <param name="Height">
    /// The height in pixels; 0 for a Scale definition with an automatic height.
    /// </param>
    /// <param name="TransformationType">Smart crop or Scale.</param>
    internal record VariantDefinition(string CodeName, string DisplayName, int Width,
        int Height, MediaTransformationType TransformationType);
}
