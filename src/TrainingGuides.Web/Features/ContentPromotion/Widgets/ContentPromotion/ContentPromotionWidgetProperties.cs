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
        Label = "{$TrainingGuides.ContentPromotionWidget.ContentSource.Label$}",
        Options = "page;{$TrainingGuides.ContentPromotionWidget.ContentSource.Options.Page$}\ncontentItem;{$TrainingGuides.ContentPromotionWidget.ContentSource.Options.ContentItem$}\nmanual;{$TrainingGuides.ContentPromotionWidget.ContentSource.Options.Manual$}",
        Order = 10)]
    public string ContentSource { get; set; } = ContentPromotionSource.PAGE;

    // Must match ContentPromotionContentTypes.PAGES, which retrieval uses. Attribute arguments must
    // be constants, so the list cannot be shared; ContentPromotionContentTypesTests checks it.
    [ContentItemSelectorComponent(
        [
            ArticlePage.CONTENT_TYPE_NAME,
            ProductPage.CONTENT_TYPE_NAME,
            ServicePage.CONTENT_TYPE_NAME
        ],
        Label = "{$TrainingGuides.ContentPromotionWidget.SelectedPage.Label$}",
        ExplanationText = "{$TrainingGuides.ContentPromotionWidget.SelectedPage.ExplanationText$}",
        MaximumItems = 1,
        Order = 20)]
    [VisibleIfEqualTo(nameof(ContentSource), ContentPromotionSource.PAGE, StringComparison.OrdinalIgnoreCase)]
    public IEnumerable<ContentItemReference> SelectedPage { get; set; } = [];

    [ContentItemSelectorComponent(
        [
            GeneralArticle.CONTENT_TYPE_NAME,
            Service.CONTENT_TYPE_NAME,
            CatFood.CONTENT_TYPE_NAME,
            DogCollar.CONTENT_TYPE_NAME,
            CatFoodVariant.CONTENT_TYPE_NAME,
            DogCollarVariant.CONTENT_TYPE_NAME
        ],
        Label = "{$TrainingGuides.ContentPromotionWidget.SelectedContentItem.Label$}",
        MaximumItems = 1,
        Order = 30)]
    [VisibleIfEqualTo(nameof(ContentSource), ContentPromotionSource.CONTENT_ITEM, StringComparison.OrdinalIgnoreCase)]
    public IEnumerable<ContentItemReference> SelectedContentItem { get; set; } = [];

    [TextInputComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.Title.Label$}",
        ExplanationText = "{$TrainingGuides.ContentPromotionWidget.Title.ExplanationText$}",
        Order = 40)]
    public string Title { get; set; } = string.Empty;

    [TextAreaComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.Description.Label$}",
        ExplanationText = "{$TrainingGuides.ContentPromotionWidget.Description.ExplanationText$}",
        Order = 50)]
    public string Description { get; set; } = string.Empty;

    [ContentItemSelectorComponent(
        Asset.CONTENT_TYPE_NAME,
        Label = "{$TrainingGuides.ContentPromotionWidget.Image.Label$}",
        ExplanationText = "{$TrainingGuides.ContentPromotionWidget.Image.ExplanationText$}",
        MaximumItems = 1,
        Order = 60)]
    public IEnumerable<ContentItemReference> Image { get; set; } = [];

    [CheckBoxComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.ShowExtras.Label$}",
        ExplanationText = "{$TrainingGuides.ContentPromotionWidget.ShowExtras.ExplanationText$}",
        Order = 80)]
    [VisibleIfNotEqualTo(nameof(ContentSource), ContentPromotionSource.MANUAL, StringComparison.OrdinalIgnoreCase)]
    public bool ShowExtras { get; set; }

    [TextInputComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.CallToActionText.Label$}",
        ExplanationText = "{$TrainingGuides.ContentPromotionWidget.CallToActionText.ExplanationText$}",
        Order = 90)]
    public string CallToActionText { get; set; } = string.Empty;

    [ContentItemSelectorComponent(
        [
            ArticlePage.CONTENT_TYPE_NAME,
            ProductPage.CONTENT_TYPE_NAME,
            ServicePage.CONTENT_TYPE_NAME
        ],
        Label = "{$TrainingGuides.ContentPromotionWidget.LinkTargetPage.Label$}",
        ExplanationText = "{$TrainingGuides.ContentPromotionWidget.LinkTargetPage.ExplanationText$}",
        MaximumItems = 1,
        Order = 100)]
    [VisibleIfNotEqualTo(nameof(ContentSource), ContentPromotionSource.PAGE, StringComparison.OrdinalIgnoreCase)]
    public IEnumerable<ContentItemReference> LinkTargetPage { get; set; } = [];

    [TextInputComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.LinkUrl.Label$}",
        ExplanationText = "{$TrainingGuides.ContentPromotionWidget.LinkUrl.ExplanationText$}",
        Order = 110)]
    [VisibleIfNotEqualTo(nameof(ContentSource), ContentPromotionSource.PAGE, StringComparison.OrdinalIgnoreCase)]
    [VisibleIfEmpty(nameof(LinkTargetPage))]
    public string LinkUrl { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.OpenInNewTab.Label$}",
        Order = 120)]
    public bool OpenInNewTab { get; set; }

    [TextInputComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.TrackingValue.Label$}",
        ExplanationText = "{$TrainingGuides.ContentPromotionWidget.TrackingValue.ExplanationText$}",
        Order = 130)]
    public string TrackingValue { get; set; } = string.Empty;

    [GeneralSelectorComponent(
        dataProviderType: typeof(HideElementsDataProvider),
        Label = "{$TrainingGuides.ContentPromotionWidget.HideElements.Label$}",
        ExplanationText = "{$TrainingGuides.ContentPromotionWidget.HideElements.ExplanationText$}",
        Placeholder = "{$TrainingGuides.ContentPromotionWidget.HideElements.Placeholder$}",
        Order = 70)]
    [VisibleIfNotEqualTo(nameof(ContentSource), ContentPromotionSource.MANUAL, StringComparison.OrdinalIgnoreCase)]
    public IEnumerable<string> HideElements { get; set; } = [];

    /// <summary>
    /// True when the element is hidden. Never in manual mode: the field is not shown there, so a
    /// value stored earlier must not hide what the editor typed.
    /// </summary>
    public bool IsElementHidden(string element) =>
        !IsContentSource(ContentPromotionSource.MANUAL) && HideElements.Contains(element);

    /// <summary>
    /// Compares the content source ignoring case, as the form's visibility conditions do.
    /// </summary>
    public bool IsContentSource(string source) =>
        string.Equals(ContentSource, source, StringComparison.OrdinalIgnoreCase);

    // Advanced styling, behind one toggle so the form stays short.
    [CheckBoxComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.ShowAdvanced.Label$}",
        Order = 140)]
    public bool ShowAdvanced { get; set; }

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.CardDesign.Label$}",
        DataProviderType = typeof(DropdownEnumOptionProvider<CardDesignOption>),
        Order = 150)]
    public string CardDesign { get; set; } = nameof(CardDesignOption.Standard);

    // Image overlay and gradient paint their own background. Both conditions must hold.
    [VisibleIfTrue(nameof(ShowAdvanced))]
    [VisibleIfNotEqualTo(nameof(CardDesign), nameof(CardDesignOption.ImageOverlay), StringComparison.OrdinalIgnoreCase)]
    [VisibleIfNotEqualTo(nameof(CardDesign), nameof(CardDesignOption.Gradient), StringComparison.OrdinalIgnoreCase)]
    [DropDownComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.ColorScheme.Label$}",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 160)]
    public string ColorScheme { get; set; } = nameof(ColorSchemeOption.Light1);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.ColumnLayout.Label$}",
        ExplanationText = "{$TrainingGuides.ContentPromotionWidget.ColumnLayout.ExplanationText$}",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColumnLayoutOption>),
        Order = 170)]
    public string ColumnLayout { get; set; } = nameof(ColumnLayoutOption.OneColumn);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.TextAlignment.Label$}",
        DataProviderType = typeof(DropdownEnumOptionProvider<TextAlignmentOption>),
        Order = 180)]
    public string TextAlignment { get; set; } = nameof(TextAlignmentOption.Left);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.CornerStyle.Label$}",
        DataProviderType = typeof(DropdownEnumOptionProvider<CornerStyleOption>),
        Order = 190)]
    public string CornerStyle { get; set; } = nameof(CornerStyleOption.Sharp);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "{$TrainingGuides.ContentPromotionWidget.CallToActionStyle.Label$}",
        DataProviderType = typeof(DropdownEnumOptionProvider<LinkStyleOption>),
        Order = 200)]
    public string CallToActionStyle { get; set; } = nameof(LinkStyleOption.Medium);
}
