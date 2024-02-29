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
        Label = "Display product features",
        Order = 20)]
    public bool ShowProductFeatures { get; set; } = true;

    [CheckBoxComponent(
        Label = "Show product image",
        Order = 30)]
    public bool ShowProductImage { get; set; } = true;

    // on click properties
    [CheckBoxComponent(
        Label = "Open Product page on click",
        ExplanationText = "If checked, opens the selected Product page detail when visitor clicks the widget.",
        Order = 40)]
    public bool OpenProductPageOnClick { get; set; } = true;

    [TextInputComponent(
        Label = "Call to action",
        ExplanationText = "Add a call to action text, e.g., \"Read more\".",
        Order = 50)]
    [VisibleIfTrue(nameof(OpenProductPageOnClick))]
    public string CallToAction { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "Open Product page in new tab",
        Order = 60)]
    [VisibleIfTrue(nameof(OpenProductPageOnClick))]
    public bool OpenInNewTab { get; set; } = true;

    // advanced styling configuration
    [CheckBoxComponent(
        Label = "Show advanced configuration options",
        Order = 70)]
    public bool ShowAdvanced { get; set; } = false;

    [DropDownComponent(
        Label = "Card size",
        DataProviderType = typeof(DropdownEnumOptionProvider<CardSizeOption>),
        Order = 80)]
    [VisibleIfTrue(nameof(ShowAdvanced))]
    public string CardSize { get; set; } = null!;

    [DropDownComponent(
        Label = "Color scheme",
        ExplanationText = "Select widget color scheme.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 90)]
    [VisibleIfTrue(nameof(ShowAdvanced))]
    public string? ColorScheme { get; set; }

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "Corner style",
        DataProviderType = typeof(DropdownEnumOptionProvider<CornerStyleOption>),
        Order = 100)]
    public string? CornerStyle { get; set; }

    [DropDownComponent(
        Label = "Content alignment",
        DataProviderType = typeof(DropdownEnumOptionProvider<ContentAlignmentOption>),
        Order = 110)]
    [VisibleIfTrue(nameof(ShowAdvanced))]
    public string ContentAlignment { get; set; } = null!;
}

public enum ContentAlignmentOption
{
    Left,
    Center,
    Right
}

public enum CardSizeOption
{
    Full = 1,
    Wide = 2,
    Middle = 3,
    Slim = 4
}