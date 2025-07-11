using System.ComponentModel;
using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;

namespace TrainingGuides.Web.Features.LandingPages.Widgets.HeroBanner;

public class HeroBannerWidgetProperties : IWidgetProperties
{
    [RadioGroupComponent(
        Label = "Selected hero widget mode",
        Options = "currentProductPage;Use the current product page\nproductPage;Select a product page\nheroContentItem;Select a hero content item from Content hub",
        Order = 10)]
    public string Mode { get; set; } = "currentProductPage";

    // The following three properties are visible based on the selected Mode.
    // The user can only one of them at a time which is why they have the same Order number.
    [VisibleIfEqualTo(nameof(Mode), "currentProductPage", StringComparison.OrdinalIgnoreCase)]
    [TextInputComponent(
        Label = "Product page anchor",
        Order = 20)]
    public string ProductPageAnchor { get; set; } = string.Empty;

    [VisibleIfEqualTo(nameof(Mode), "productPage", StringComparison.OrdinalIgnoreCase)]
    [ContentItemSelectorComponent(
        TrainingGuides.ProductPage.CONTENT_TYPE_NAME,
        Label = "Selected product",
        MaximumItems = 1,
        ExplanationText = "The widget will display the default product's content.",
        Order = 20)]
    public IEnumerable<ContentItemReference> ProductPage { get; set; } = Enumerable.Empty<ContentItemReference>();

    [VisibleIfEqualTo(nameof(Mode), "heroContentItem", StringComparison.OrdinalIgnoreCase)]
    [ContentItemSelectorComponent(
        TrainingGuides.Hero.CONTENT_TYPE_NAME,
        Label = "Select hero banner",
        ExplanationTextAsHtml = true,
        ExplanationText = "The widget will display the content from the selected Hero content item.",
        Order = 20)]
    public IEnumerable<ContentItemReference> Hero { get; set; } = Enumerable.Empty<ContentItemReference>();

    [VisibleIfEqualTo(nameof(Mode), "productPage", StringComparison.OrdinalIgnoreCase)]
    [TextInputComponent(
        Label = "Product page anchor",
        Order = 30)]
    public string SelectedProductPageAnchor { get; set; } = string.Empty;

    [TextInputComponent(
        Label = "CTA text",
        ExplanationText = "Text of the call to action. (Overrides that of the selected content item)",
        Order = 40)]
    public string CTA { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "Display CTA",
        ExplanationText = "When selected, the banner widget displays call to action.",
        Order = 50)]
    public bool DisplayCTA { get; set; } = true;

    [CheckBoxComponent(
        Label = "Open in new tab",
        Order = 60)]
    public bool OpenInNewTab { get; set; } = false;

    [CheckBoxComponent(
        Label = "Show design options",
        Order = 70)]
    public bool ChangeDesign { get; set; } = false;

    [DropDownComponent(
        Label = "Text color",
        ExplanationText = "Select the color of the component text. Changing this will automatically change the component color theme to fit the text color.",
        DataProviderType = typeof(DropdownEnumOptionProvider<TextColorOption>),
        Order = 80)]
    [VisibleIfTrue(nameof(ChangeDesign))]
    public string TextColor { get; set; } = TextColorOption.Dark.ToString();

    [CheckBoxComponent(
        Label = "Show benefits",
        ExplanationText = "Displays product benefits if selected.",
        Order = 90)]
    [VisibleIfTrue(nameof(ChangeDesign))]
    public bool ShowBenefits { get; set; } = false;

    [CheckBoxComponent(
        Label = "Show image",
        Order = 100)]
    [VisibleIfTrue(nameof(ChangeDesign))]
    public bool ShowImage { get; set; } = true;

    [RadioGroupComponent(
        Label = "Display image",
        Options = "full;In the background (Shows image in the banner's background. If empty, the image displays on the right.)\ncircle;In a circle (Shows the banner's image in a circle with additional graphic elements.)",
        Order = 110)]
    [VisibleIfTrue(nameof(ChangeDesign))]
    public string Width { get; set; } = "circle";
}

public enum TextColorOption
{
    [Description("Dark")]
    Dark,
    [Description("Light")]
    Light
}