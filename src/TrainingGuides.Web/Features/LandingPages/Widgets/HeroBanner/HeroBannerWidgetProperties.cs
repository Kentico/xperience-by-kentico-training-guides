using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;

namespace TrainingGuides.Web.Features.LandingPages.Widgets.HeroBanner;

public class HeroBannerWidgetProperties : IWidgetProperties
{
    [RadioGroupComponent(
        Label = "Selected hero widget mode",
        Options = "currentProductPage;Use the current product page\nproductPage;Select a product page\nheroContentItem;Select a hero content item from Content hub",
        Order = 1
    )]
    public string Mode { get; set; } = "currentProductPage";

    [VisibleIfEqualTo(nameof(Mode), "currentProductPage", StringComparison.OrdinalIgnoreCase)]
    [TextInputComponent(Label = "Product page anchor", Order = 1)]
    public string ProductPageAnchor { get; set; } = null!;

    [VisibleIfEqualTo(nameof(Mode), "productPage", StringComparison.OrdinalIgnoreCase)]
    [WebPageSelectorComponent(Label = "Selected product", Order = 1, MaximumPages = 1, Sortable = true,
        ExplanationText =
            "The widget will display the default product's content.")]
    public IEnumerable<WebPageRelatedItem> ProductPage { get; set; } = null!;

    [VisibleIfEqualTo(nameof(Mode), "productPage", StringComparison.OrdinalIgnoreCase)]
    [TextInputComponent(Label = "Product page anchor", Order = 2)]
    public string SelectedProductPageAnchor { get; set; } = null!;

    [VisibleIfEqualTo(nameof(Mode), "heroContentItem", StringComparison.OrdinalIgnoreCase)]
    [ContentItemSelectorComponent(TrainingGuides.Hero.CONTENT_TYPE_NAME, Label = "Select hero banner", Order = 3,
        ExplanationTextAsHtml = true,
        ExplanationText =
            "The widget will display the content from the selected Hero content item.")]
    public IEnumerable<ContentItemReference> Hero { get; set; } = null!;

    [TextInputComponent(Label = "CTA Text", Order = 5, ExplanationText = "Text of the call to action. (Overrides that of the selected content item)")]
    public string CTA { get; set; } = null!;

    [CheckBoxComponent(Label = "Display CTA", Order = 8, ExplanationText = "When selected, the banner widget displays call to action")]
    public bool DisplayCTA { get; set; } = true;

    [CheckBoxComponent(Label = "Open in new tab", Order = 9)]
    public bool OpenInNewTab { get; set; } = false;

    [CheckBoxComponent(Label = "Show design options", Order = 10)]
    public bool ChangeDesign { get; set; } = false;

    [CheckBoxComponent(Label = "Show benefits", Order = 11, ExplanationText = "Displays product benefits if selected")]
    [VisibleIfTrue(nameof(ChangeDesign))]
    public bool ShowBenefits { get; set; } = false;

    [CheckBoxComponent(Label = "Show image", Order = 12)]
    [VisibleIfTrue(nameof(ChangeDesign))]
    public bool ShowImage { get; set; } = true;

    [RadioGroupComponent(Label = "Display image", Options = "full;In the background (Shows image in the banner's background. If empty, the image displays on the right.)\ncircle;In a circle (Shows the banner's image in a circle with additional graphic elements.)", Order = 13)]
    [VisibleIfTrue(nameof(ChangeDesign))]
    public string Width { get; set; } = "circle";

    [DropDownComponent(Label = "Text color", Order = 10, Options = "dark;Dark\nlight;Light", ExplanationText = "Select the color of the component text. Changing this will automatically change the component color theme to fit the text color.")]
    [VisibleIfTrue(nameof(ChangeDesign))]
    public string TextColor { get; set; } = "dark";
}
