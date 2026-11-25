using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.ResponsiveImages.ImageWidget;

/// <summary>
/// Configuration properties for the "Image" widget.
/// </summary>
public class ImageWidgetProperties : IWidgetProperties
{
    /// <summary>
    /// Reference to the selected Asset content item.
    /// </summary>
    [ContentItemSelectorComponent(Asset.CONTENT_TYPE_NAME,
        Label = "Image", MaximumItems = 1, Order = 10)]
    public IEnumerable<ContentItemReference> Image { get; set; } = [];

    /// <summary>
    /// Code name of the selected image variant, or <c>null</c> for the responsive width series.
    /// </summary>
    [DropDownComponent(Label = "Image variant",
        DataProviderType = typeof(ImageVariantOptionsProvider),
        Placeholder = ImageVariantOptionsProvider.AUTOMATIC_OPTION_TEXT,
        ExplanationText = "With Automatic, the browser downloads the size that fits "
            + "each screen. With a variant, the image has that variant's crop and size "
            + "on every screen. An image that lacks the variant shows as Automatic.",
        Order = 20)]
    public string? ImageVariant { get; set; }
}
