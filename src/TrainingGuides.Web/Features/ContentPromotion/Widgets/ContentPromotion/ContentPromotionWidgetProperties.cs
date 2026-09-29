using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;
using TrainingGuides.Web.Features.Shared.OptionProviders.TextAlignment;

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

    [TextInputComponent(
        Label = "Tracking value",
        ExplanationText = "Logged as the value of the \"Content promotion click\" activity when a visitor clicks the card. Leave empty to track nothing.",
        Order = 130)]
    public string TrackingValue { get; set; } = string.Empty;

    // NOTE: the admin form component for this property is not settled yet. The resolution
    // logic only needs the selected element names, so it is deliberately left as a plain
    // collection until the component is confirmed against the Kentico docs.
    public IEnumerable<string> HideElements { get; set; } = [];

    // Advanced styling. Everything below hides behind the one toggle, so the form stays short
    // for the editors who only want a card.
    [CheckBoxComponent(
        Label = "Adjust design",
        Order = 140)]
    public bool ShowAdvanced { get; set; }

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "Card design",
        DataProviderType = typeof(DropdownEnumOptionProvider<CardDesignOption>),
        Order = 150)]
    public string CardDesign { get; set; } = nameof(CardDesignOption.Standard);

    // Image overlay and gradient paint their own background, so a color scheme on top of them
    // would only fight it. The two conditions AND together.
    [VisibleIfTrue(nameof(ShowAdvanced))]
    [VisibleIfNotEqualTo(nameof(CardDesign), nameof(CardDesignOption.ImageOverlay), StringComparison.OrdinalIgnoreCase)]
    [VisibleIfNotEqualTo(nameof(CardDesign), nameof(CardDesignOption.Gradient), StringComparison.OrdinalIgnoreCase)]
    [DropDownComponent(
        Label = "Color scheme",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 160)]
    public string ColorScheme { get; set; } = nameof(ColorSchemeOption.Light1);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "Column layout",
        ExplanationText = "How the card splits between image and text. A card has two regions, so the three-column options lay out the same as two even columns.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColumnLayoutOption>),
        Order = 170)]
    public string ColumnLayout { get; set; } = nameof(ColumnLayoutOption.OneColumn);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "Text alignment",
        DataProviderType = typeof(DropdownEnumOptionProvider<TextAlignmentOption>),
        Order = 180)]
    public string TextAlignment { get; set; } = nameof(TextAlignmentOption.Left);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "Corner style",
        DataProviderType = typeof(DropdownEnumOptionProvider<CornerStyleOption>),
        Order = 190)]
    public string CornerStyle { get; set; } = nameof(CornerStyleOption.Sharp);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "Call to action style",
        DataProviderType = typeof(DropdownEnumOptionProvider<LinkStyleOption>),
        Order = 200)]
    public string CallToActionStyle { get; set; } = nameof(LinkStyleOption.Medium);
}
