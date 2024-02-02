using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;

namespace Kbank.Web.Components.Widgets.HeroBannerWidget;

public class HeroBannerWidgetProperties : IWidgetProperties
{
    [RadioGroupComponent(
        Label = "Selected hero widget mode",
        Options = "currentProductPage;Current product page\nproductPage;Select product page\nheroContentItem;Select hero content item",
        Order = 1
    )]
    public string Mode { get; set; } = "currentProductPage";

    [VisibleIfEqualTo(nameof(Mode), "currentProductPage", StringComparison.OrdinalIgnoreCase)]
    [TextInputComponent(Label = "Product page anchor", Order = 1)]
    public string ProductPageAnchor { get; set; } = null!;

    [VisibleIfEqualTo(nameof(Mode), "productPage", StringComparison.OrdinalIgnoreCase)]
    [WebPageSelectorComponent(Label = "Selected product", Order = 1, MaximumPages = 1, Sortable = true,
        ExplanationText =
            "The widget will display the default product's content (Header, Short description, Media, Link (optional), and Product benefits).")]
    public IEnumerable<WebPageRelatedItem> ProductPage { get; set; } = null!;

    [VisibleIfEqualTo(nameof(Mode), "productPage", StringComparison.OrdinalIgnoreCase)]
    [TextInputComponent(Label = "Product page anchor", Order = 2)]
    public string SelectedProductPageAnchor { get; set; } = null!;

    [VisibleIfEqualTo(nameof(Mode), "heroContentItem", StringComparison.OrdinalIgnoreCase)]
    [ContentItemSelectorComponent(TrainingGuides.Hero.CONTENT_TYPE_NAME, Label = "Select hero banner", Order = 3,
        ExplanationTextAsHtml = true,
        ExplanationText =
            "The widget will display the content from the selected Hero content type (Header, Short description, Media, Link (optional), and Product benefits).<br/>Adding a custom banner keeps the Product page's URL but overrides the product's default values (Header, Short description, Media, and Product benefits)")]
    public IEnumerable<ContentItemReference> Hero { get; set; } = null!;

    [VisibleIfEqualTo(nameof(Mode), "heroContentItem", StringComparison.OrdinalIgnoreCase)]
    [UrlSelectorComponent(Label = "Banner Url", Order = 4)]
    public string BannerUrl { get; set; } = null!;

    [TextInputComponent(Label = "CTAText", Order = 5)]
    public string CTA { get; set; } = null!;

    [CheckBoxComponent(Label = "Add absolute URL", Order = 6)]
    public bool CustomAbsoluteUrl { get; set; } = false;

    [TextInputComponent(Label = "Absolute url", Order = 7, ExplanationText = "Add a hyperlink to an external site, or use the product's URL + anchor tag # for referencing an anchor on the page, for example, \"https://kbank.com/loans/optimal-loan#form\"")]
    [VisibleIfTrue(nameof(CustomAbsoluteUrl))]
    public string AbsoluteUrl { get; set; } = null!;

    [CheckBoxComponent(Label = "Display CTAText", Order = 8, ExplanationText = "When selected, the banner widget displays call to action")]
    public bool DisplayCTA { get; set; } = true;

    [CheckBoxComponent(Label = "Open in new tab", Order = 9)]
    public bool OpenInNewTab { get; set; } = false;

    [CheckBoxComponent(Label = "Change widget design", Order = 10)]
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

    [DropDownComponent(Label = "Text Color", Order = 10, Options = "dark;Dark\nlight;Light", ExplanationText = "Select the color of the component text. Changing this will automatically change the component color theme to fit the text color.")]
    [VisibleIfTrue(nameof(ChangeDesign))]
    public string TextColor { get; set; } = "dark";
}
