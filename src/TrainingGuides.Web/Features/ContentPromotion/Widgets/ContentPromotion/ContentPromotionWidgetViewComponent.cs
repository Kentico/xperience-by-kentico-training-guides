using CMS.Helpers;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TrainingGuides.Web.Features.ContentPromotion.Models;
using TrainingGuides.Web.Features.ContentPromotion.Services;
using TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColumnLayout;
using TrainingGuides.Web.Features.Shared.OptionProviders.TextAlignment;
using TrainingGuides.Web.Features.Shared.Services;

[assembly:
    RegisterWidget(
        identifier: ContentPromotionWidgetViewComponent.IDENTIFIER,
        viewComponentType: typeof(ContentPromotionWidgetViewComponent),
        name: "{$TrainingGuides.ContentPromotionWidget.Name$}",
        propertiesType: typeof(ContentPromotionWidgetProperties),
        Description = "{$TrainingGuides.ContentPromotionWidget.Description$}",
        IconClass = "icon-megaphone")]

namespace TrainingGuides.Web.Features.ContentPromotion.Widgets.ContentPromotion;

public class ContentPromotionWidgetViewComponent(
    IContentPromotionService contentPromotionService,
    IComponentStyleEnumService componentStyleEnumService) : ViewComponent
{
    public const string IDENTIFIER = "TrainingGuides.ContentPromotionWidget";

    /// <summary>
    /// The code name of the custom activity type logged when a visitor clicks the card. Created in
    /// the admin UI and serialized under <c>App_Data/CIRepository/@global/om.activitytype</c>.
    /// </summary>
    public const string ACTIVITY_IDENTIFIER = "contentpromotionclick";

    private const string CARD_BASE_CLASS = "c-content-promotion";
    private const string CONTENT_BASE_CLASS = "c-content-promotion__content";
    private const string IMAGE_BASE_CLASS = "c-content-promotion__image";
    private const string BS_CARD_CLASSES = "c-card md";
    private const string BS_BUTTON_CLASSES = "btn text-uppercase";

    public async Task<ViewViewComponentResult> InvokeAsync(ContentPromotionWidgetProperties properties)
    {
        var model = await BuildWidgetViewModel(properties);

        return View("~/Features/ContentPromotion/Widgets/ContentPromotion/ContentPromotionWidget.cshtml", model);
    }

    public async Task<ContentPromotionWidgetViewModel> BuildWidgetViewModel(ContentPromotionWidgetProperties properties)
    {
        var promotedItem = await contentPromotionService.ResolvePromotedItem(properties);

        return new ContentPromotionWidgetViewModel
        {
            SelectionFailed = promotedItem.SelectionFailed,
            CallToActionHidden = properties.HideElements.Contains(ContentPromotionElement.CALL_TO_ACTION),
            DisplayValues = contentPromotionService.ResolveDisplayValues(
                properties,
                promotedItem.Item,
                await contentPromotionService.ResolveOverrideImage(properties)),
            Link = await contentPromotionService.ResolveLink(properties, promotedItem.Page),
            Extras = await contentPromotionService.ResolveExtras(properties, promotedItem),
            TrackingValue = properties.TrackingValue ?? string.Empty,
            CornerStyle = properties.CornerStyle ?? string.Empty,
            CardCssClasses = GetCardCssClasses(properties).Join(" "),
            ContentCssClasses = GetContentCssClasses(properties).Join(" "),
            ImageCssClasses = IMAGE_BASE_CLASS,
            CallToActionCssClasses = GetCallToActionCssClasses(properties).Join(" ")
        };
    }

    private List<string> GetCardCssClasses(ContentPromotionWidgetProperties properties)
    {
        var cardDesign = GetCardDesign(properties);

        List<string> cssClasses =
        [
            CARD_BASE_CLASS,
            GetCardDesignClass(cardDesign),
            BS_CARD_CLASSES,
            GetColumnLayoutClass(properties)
        ];

        // Image overlay and gradient paint their own background. The color scheme property is
        // hidden for both in the admin UI, but a previously chosen value is still stored on the
        // widget, so the design - not the stored value - decides whether it is applied.
        if (!PaintsOwnBackground(cardDesign))
        {
            cssClasses.AddRange(
                componentStyleEnumService.GetColorSchemeClasses(
                    componentStyleEnumService.GetColorScheme(properties.ColorScheme ?? string.Empty)));
        }

        cssClasses.AddRange(
            componentStyleEnumService.GetCornerStyleClasses(
                componentStyleEnumService.GetCornerStyle(properties.CornerStyle ?? string.Empty)));

        return [.. cssClasses.Where(cssClass => !string.IsNullOrWhiteSpace(cssClass))];
    }

    private List<string> GetContentCssClasses(ContentPromotionWidgetProperties properties)
    {
        const string TEXT_ALIGN_LEFT_CLASS = "align-left";
        const string TEXT_ALIGN_CENTER_CLASS = "align-center";
        const string TEXT_ALIGN_RIGHT_CLASS = "align-right";

        string alignmentClass = properties.TextAlignment switch
        {
            nameof(TextAlignmentOption.Center) => TEXT_ALIGN_CENTER_CLASS,
            nameof(TextAlignmentOption.Right) => TEXT_ALIGN_RIGHT_CLASS,
            _ => TEXT_ALIGN_LEFT_CLASS
        };

        return [CONTENT_BASE_CLASS, alignmentClass];
    }

    private List<string> GetCallToActionCssClasses(ContentPromotionWidgetProperties properties)
    {
        List<string> cssClasses = [BS_BUTTON_CLASSES];

        cssClasses.AddRange(
            componentStyleEnumService.GetColorSchemeClasses(
                componentStyleEnumService.GetLinkStyle(properties.CallToActionStyle ?? string.Empty)));

        return [.. cssClasses.Where(cssClass => !string.IsNullOrWhiteSpace(cssClass))];
    }

    private static CardDesignOption GetCardDesign(ContentPromotionWidgetProperties properties) =>
        Enum.TryParse(properties.CardDesign, true, out CardDesignOption cardDesign)
            ? cardDesign
            : CardDesignOption.Standard;

    /// <summary>
    /// True for the designs that supply their own card background, and so leave no room for a
    /// color scheme on top of it.
    /// </summary>
    private static bool PaintsOwnBackground(CardDesignOption cardDesign) =>
        cardDesign is CardDesignOption.ImageOverlay or CardDesignOption.Gradient;

    private static string GetCardDesignClass(CardDesignOption cardDesign) => cardDesign switch
    {
        CardDesignOption.ImageOverlay => $"{CARD_BASE_CLASS}--image-overlay",
        CardDesignOption.Gradient => $"{CARD_BASE_CLASS}--gradient",
        CardDesignOption.Spotlight => $"{CARD_BASE_CLASS}--spotlight",
        CardDesignOption.Portrait => $"{CARD_BASE_CLASS}--portrait",
        _ => $"{CARD_BASE_CLASS}--standard"
    };

    /// <summary>
    /// A card has two regions - image and text - so the three-column options lay out the same as
    /// two even columns rather than inventing a third region that has nothing to put in it.
    /// </summary>
    private static string GetColumnLayoutClass(ContentPromotionWidgetProperties properties) =>
        properties.ColumnLayout switch
        {
            nameof(ColumnLayoutOption.TwoColumnLgSm) => $"{CARD_BASE_CLASS}--split-lg-sm",
            nameof(ColumnLayoutOption.TwoColumnSmLg) => $"{CARD_BASE_CLASS}--split-sm-lg",
            nameof(ColumnLayoutOption.TwoColumnEven)
                or nameof(ColumnLayoutOption.ThreeColumnEven)
                or nameof(ColumnLayoutOption.ThreeColumnSmLgSm) => $"{CARD_BASE_CLASS}--split-even",
            _ => $"{CARD_BASE_CLASS}--stacked"
        };
}
