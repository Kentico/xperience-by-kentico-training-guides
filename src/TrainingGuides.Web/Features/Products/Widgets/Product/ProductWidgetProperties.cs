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
    [RadioGroupComponent(
        Label = "Select which Product page to display",
        Options = $"{ProductWidgetMode.CURRENT_PAGE};{ProductWidgetMode.CURRENT_PAGE_DESCRIPTION}"
            + $"\n{ProductWidgetMode.SELECT_PAGE};{ProductWidgetMode.SELECT_PAGE_DESCRIPTION}",
        Order = 10
    )]
    public string Mode { get; set; } = ProductWidgetMode.CURRENT_PAGE;

    [VisibleIfEqualTo(nameof(Mode), ProductWidgetMode.CURRENT_PAGE, StringComparison.OrdinalIgnoreCase)]
    [TextInputComponent(
        Label = "Product page anchor",
        ExplanationText = "If displaying current page, optionally set what page anchor to navigate to when visitor clicks the widget or its call to action link.",
        Order = 20)]
    public string? PageAnchor { get; set; }

    [VisibleIfEqualTo(nameof(Mode), ProductWidgetMode.SELECT_PAGE, StringComparison.OrdinalIgnoreCase)]
    [WebPageSelectorComponent(
        Label = "Select product page",
        ExplanationText = "Choose the product page to be dispayed in the widget.",
        Order = 30)]
    public IEnumerable<WebPageRelatedItem> SelectedProductPage { get; set; } = new List<WebPageRelatedItem>();

    [CheckBoxComponent(
        Label = "Display product image",
        Order = 40)]
    public bool ShowProductImage { get; set; } = true;

    [CheckBoxComponent(
        Label = "Display product benefits",
        Order = 50)]

    public bool ShowProductBenefits { get; set; } = true;
    [CheckBoxComponent(
        Label = "Display product features",
        Order = 60)]
    public bool ShowProductFeatures { get; set; } = false;

    [TextInputComponent(
        Label = "Call to action (CTA) text",
        ExplanationText = "Add a call to action text, e.g., \"Read more\".",
        Order = 70)]
    public string CallToAction { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "Open in new tab",
        ExplanationText = "Opens Product page in new tab when visitor clicks the widget or CTA",
        Order = 80)]
    public bool OpenInNewTab { get; set; } = true;

    // advanced styling configuration
    [CheckBoxComponent(
        Label = "Show advanced options",
        Order = 90)]
    public bool ShowAdvanced { get; set; } = false;

    [DropDownComponent(
        Label = "Color scheme",
        ExplanationText = "Select widget color scheme.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 100)]
    [VisibleIfTrue(nameof(ShowAdvanced))]
    public string? ColorScheme { get; set; } = nameof(ColorSchemeOption.Dark1);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "Corner style",
        DataProviderType = typeof(DropdownEnumOptionProvider<CornerStyleOption>),
        Order = 110)]
    public string? CornerStyle { get; set; } = nameof(CornerStyleOption.Sharp);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [CheckBoxComponent(
        Label = "Drop shadow",
        Order = 120)]
    public bool DropShadow { get; set; } = false;

    [DropDownComponent(
        Label = "Image position",
        ExplanationText = "Select the image position with respect to text.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ImagePositionOption>),
        Order = 130)]
    [VisibleIfTrue(nameof(ShowAdvanced))]
    [VisibleIfTrue(nameof(ShowProductImage))]
    public string? ImagePosition { get; set; } = nameof(ImagePositionOption.FullWidth);

    [DropDownComponent(
        Label = "Text alignment",
        DataProviderType = typeof(DropdownEnumOptionProvider<ContentAlignmentOption>),
        Order = 140)]
    [VisibleIfTrue(nameof(ShowAdvanced))]
    public string? TextAlignment { get; set; } = nameof(ContentAlignmentOption.Left);

    [DropDownComponent(
        Label = "CTA button style",
        DataProviderType = typeof(DropdownEnumOptionProvider<LinkStyleOption>),
        Order = 150
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

public static class ProductWidgetMode
{
    public const string CURRENT_PAGE = "currentProductPage";
    public const string CURRENT_PAGE_DESCRIPTION = "Use the current product page. (If this widget is placed on existing Product page, automatically displays the correspongind Product data.)";

    public const string SELECT_PAGE = "productPage";
    public const string SELECT_PAGE_DESCRIPTION = "Select a product page.";
}