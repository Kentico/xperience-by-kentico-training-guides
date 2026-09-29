using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.ContentPromotion.Models;

namespace TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;

public class ContentPromotionWidgetProperties : IWidgetProperties
{
    [RadioGroupComponent(
        Label = "Content source",
        Options = "page;Page\ncontentItem;Content hub item\nmanual;Manual content",
        Order = 10)]
    public string ContentSource { get; set; } = ContentPromotionSource.PAGE;

    // WORKAROUND, and the reason EmptyPage and StoreSection appear in a list of promotable
    // content: the combined content selector appears to render only those branches of the content
    // tree whose pages are all of an allowed content type. A ProductPage sits at
    // /Store/Dog-collar/<product>, under an EmptyPage and a StoreSection, so with only the three
    // promotable types listed the editor cannot expand /Store at all and no product page is
    // reachable. ServicePage has the same problem under /Products. Listing the containers makes
    // the tree walkable, at the cost of letting an editor select a page the widget cannot promote
    // - ResolvePromotedItem reports those explicitly rather than claiming they failed to load.
    // Remove both container types once the selector can be scoped to selectable types while
    // leaving the tree navigable. See the T6 notes in the ticket breakdown.
    [ContentItemSelectorComponent(
        [
            ArticlePage.CONTENT_TYPE_NAME,
            ProductPage.CONTENT_TYPE_NAME,
            ServicePage.CONTENT_TYPE_NAME,
            EmptyPage.CONTENT_TYPE_NAME,
            StoreSection.CONTENT_TYPE_NAME
        ],
        Label = "Selected page",
        ExplanationText = "Pick an article, product or service page. Section pages are listed only so you can navigate to them.",
        // Keep in step with ContentPromotionContentTypes.PAGES, which retrieval uses. An attribute
        // argument must be a compile-time constant, so the array cannot be shared directly.
        MaximumItems = 1,
        Order = 20)]
    [VisibleIfEqualTo(nameof(ContentSource), ContentPromotionSource.PAGE, StringComparison.OrdinalIgnoreCase)]
    public IEnumerable<ContentItemReference> SelectedPage { get; set; } = [];

    [ContentItemSelectorComponent(
        [
            GeneralArticle.CONTENT_TYPE_NAME,
            Interview.CONTENT_TYPE_NAME,
            Service.CONTENT_TYPE_NAME,
            CatFood.CONTENT_TYPE_NAME,
            DogCollar.CONTENT_TYPE_NAME,
            CatFoodVariant.CONTENT_TYPE_NAME,
            DogCollarVariant.CONTENT_TYPE_NAME
        ],
        Label = "Selected content item",
        MaximumItems = 1,
        Order = 30)]
    [VisibleIfEqualTo(nameof(ContentSource), ContentPromotionSource.CONTENT_ITEM, StringComparison.OrdinalIgnoreCase)]
    public IEnumerable<ContentItemReference> SelectedContentItem { get; set; } = [];

    [TextInputComponent(
        Label = "Title",
        ExplanationText = "Leave empty to use the title of the selected content item.",
        Order = 40)]
    public string Title { get; set; } = string.Empty;

    [TextAreaComponent(
        Label = "Description",
        ExplanationText = "Leave empty to use the description of the selected content item.",
        Order = 50)]
    public string Description { get; set; } = string.Empty;

    [ContentItemSelectorComponent(
        Asset.CONTENT_TYPE_NAME,
        Label = "Image",
        ExplanationText = "Leave empty to use the image of the selected content item. Alt text comes from the selected asset.",
        MaximumItems = 1,
        Order = 60)]
    public IEnumerable<ContentItemReference> Image { get; set; } = [];

    [CheckBoxComponent(
        Label = "Show type-specific extras",
        ExplanationText = "Shows article categories, service benefits, or product price and stock. Nothing is shown when the selected item has none of them.",
        Order = 70)]
    [VisibleIfNotEqualTo(nameof(ContentSource), ContentPromotionSource.MANUAL, StringComparison.OrdinalIgnoreCase)]
    public bool ShowExtras { get; set; }

    [TextInputComponent(
        Label = "Call to action text",
        ExplanationText = "Leave empty to use the call to action of the selected content item.",
        Order = 90)]
    public string CallToActionText { get; set; } = string.Empty;

    [ContentItemSelectorComponent(
        [
            ArticlePage.CONTENT_TYPE_NAME,
            ProductPage.CONTENT_TYPE_NAME,
            ServicePage.CONTENT_TYPE_NAME
        ],
        Label = "Link target",
        ExplanationText = "The page the card links to. Content hub items may have no page of their own.",
        MaximumItems = 1,
        Order = 100)]
    [VisibleIfNotEqualTo(nameof(ContentSource), ContentPromotionSource.PAGE, StringComparison.OrdinalIgnoreCase)]
    public IEnumerable<ContentItemReference> LinkTargetPage { get; set; } = [];

    [TextInputComponent(
        Label = "Link URL",
        ExplanationText = "Used only when no link target page is selected.",
        Order = 110)]
    [VisibleIfNotEqualTo(nameof(ContentSource), ContentPromotionSource.PAGE, StringComparison.OrdinalIgnoreCase)]
    [VisibleIfEmpty(nameof(LinkTargetPage))]
    public string LinkUrl { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "Open in new tab",
        Order = 120)]
    public bool OpenInNewTab { get; set; }

    // NOTE: the admin form component for this property is not settled yet. The resolution
    // logic only needs the selected element names, so it is deliberately left as a plain
    // collection until the component is confirmed against the Kentico docs.
    public IEnumerable<string> HideElements { get; set; } = [];
}
