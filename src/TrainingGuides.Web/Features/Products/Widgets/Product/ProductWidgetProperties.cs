using System.ComponentModel;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.OptionsProviders.ColorScheme;

namespace TrainingGuides.Web.Features.Products.Widgets.Product;

public class ProductWidgetProperties : IWidgetProperties
{
    [WebPageSelectorComponent(
        Label = "Select product page",
        ExplanationText = "Choose the product page to be dispayed in the widget.",
        Order = 10)]
    public IEnumerable<WebPageRelatedItem> SelectedProductPage { get; set; } = new List<WebPageRelatedItem>();

    [CheckBoxComponent(
        Label = "Display product image",
        Order = 20)]
    public bool ShowProductImage { get; set; } = true;

    [CheckBoxComponent(
        Label = "Display product benefits",
        Order = 30)]

    public bool ShowProductBenefits { get; set; } = true;
    [CheckBoxComponent(
        Label = "Display product features",
        Order = 40)]
    public bool ShowProductFeatures { get; set; } = false;

    [TextInputComponent(
        Label = "Call to action (CTA) text",
        ExplanationText = "Add a call to action text, e.g., \"Read more\".",
        Order = 50)]
    public string CallToAction { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "Open in new tab",
        ExplanationText = "Opens Product page in new tab when visitor clicks the widget or CTA",
        Order = 60)]
    public bool OpenInNewTab { get; set; } = true;

    // advanced styling configuration
    [CheckBoxComponent(
        Label = "Show advanced options",
        Order = 70)]
    public bool ShowAdvanced { get; set; } = false;

    [DropDownComponent(
        Label = "Color scheme",
        ExplanationText = "Select widget color scheme.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 80)]
    [VisibleIfTrue(nameof(ShowAdvanced))]
    public string? ColorScheme { get; set; } = nameof(ColorSchemeOption.Dark1);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "Corner style",
        DataProviderType = typeof(DropdownEnumOptionProvider<CornerStyleOption>),
        Order = 90)]
    public string? CornerStyle { get; set; } = nameof(CornerStyleOption.Sharp);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [CheckBoxComponent(
        Label = "Drop shadow",
        Order = 100)]
    public bool DropShadow { get; set; } = false;

    [DropDownComponent(
        Label = "Image position",
        ExplanationText = "Select the image position with respect to text.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ImagePositionOption>),
        Order = 110)]
    [VisibleIfTrue(nameof(ShowAdvanced))]
    [VisibleIfTrue(nameof(ShowProductImage))]
    public string? ImagePosition { get; set; } = nameof(ImagePositionOption.FullWidth);

    [DropDownComponent(
        Label = "Text alignment",
        DataProviderType = typeof(DropdownEnumOptionProvider<ContentAlignmentOption>),
        Order = 120)]
    [VisibleIfTrue(nameof(ShowAdvanced))]
    public string? TextAlignment { get; set; } = nameof(ContentAlignmentOption.Left);

    [DropDownComponent(
        Label = "CTA button style",
        DataProviderType = typeof(DropdownEnumOptionProvider<LinkStyleOption>),
        Order = 130
    )]
    [VisibleIfTrue(nameof(ShowAdvanced))]
    [VisibleIfNotEmpty(nameof(CallToAction))]
    public string? CallToActionStyle { get; set; } = nameof(LinkStyleOption.Medium);
}

public enum ImagePositionOption
{
    [Description("Full width")]
    FullWidth,
    [Description("Ascending shape")]
    Ascending,
    [Description("Descending shape")]
    Descending
}

public enum ContentAlignmentOption
{
    Left,
    Center,
    Right
}