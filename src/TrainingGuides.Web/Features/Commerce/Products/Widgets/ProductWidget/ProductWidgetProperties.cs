using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace TrainingGuides.Web.Features.Commerce.Products.Widgets.ProductWidget;

public class ProductWidgetProperties : IWidgetProperties
{
    [CheckBoxComponent(
        Label = "Display current page product",
        ExplanationText = "When checked, displays the product from the current product page. When unchecked, allows you to select a specific product.",
        Order = 10)]
    public bool DisplayCurrentPage { get; set; } = true;

    [ContentItemSelectorComponent(
        ProductPage.CONTENT_TYPE_NAME,
        Label = "Selected product page",
        ExplanationText = "Select the product page to display.",
        MaximumItems = 1,
        Order = 20)]
    [VisibleIfEqualTo(nameof(DisplayCurrentPage), false)]
    public IEnumerable<ContentItemReference> SelectedProduct { get; set; } = [];

    [CheckBoxComponent(
        Label = "Show variant selection",
        ExplanationText = "When checked, displays variant options that visitors can select.",
        Order = 30)]
    public bool ShowVariantSelection { get; set; } = true;

    [CheckBoxComponent(
        Label = "Show variant details",
        ExplanationText = "When checked, displays the variant description and other details beneath the main product description.",
        Order = 35)]
    public bool ShowVariantDetails { get; set; } = true;

    [CheckBoxComponent(
        Label = "Show call to action",
        ExplanationText = "When checked, displays a button linking to the product page.",
        Order = 40)]
    public bool ShowCallToAction { get; set; } = false;

    [TextInputComponent(
        Label = "Call to action text",
        ExplanationText = "The text displayed on the call-to-action button.",
        Order = 50)]
    [VisibleIfEqualTo(nameof(ShowCallToAction), true)]
    public string CallToActionText { get; set; } = "View Product";
}
