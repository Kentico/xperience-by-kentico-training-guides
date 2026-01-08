using System.ComponentModel;
using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;
using TrainingGuides.Web.Features.Shared.OptionProviders.ColorScheme;
using TrainingGuides.Web.Features.Shared.OptionProviders.CornerStyle;

namespace TrainingGuides.Web.Features.FinancialServices.Widgets.Service;

// NOTE: For an example of localizing widget properties (labels, explanation texts, and options),
// see CallToActionWidgetProperties in Features/LandingPages/Widgets/CallToAction/

public class ServiceWidgetProperties : IWidgetProperties
{
    [RadioGroupComponent(
        Label = "Select which Service page to display",
        Options = $"{ServiceWidgetModel.CURRENT_PAGE};{ServiceWidgetModel.CURRENT_PAGE_DESCRIPTION}"
            + $"\n{ServiceWidgetModel.SELECT_PAGE};{ServiceWidgetModel.SELECT_PAGE_DESCRIPTION}",
        Order = 10)]
    public string Mode { get; set; } = ServiceWidgetModel.CURRENT_PAGE;

    [VisibleIfEqualTo(nameof(Mode), ServiceWidgetModel.CURRENT_PAGE, StringComparison.OrdinalIgnoreCase)]
    [TextInputComponent(
        Label = "Service page anchor",
        ExplanationText = "If displaying current page, optionally set what page anchor to navigate to when visitor clicks the widget or its call to action link.",
        Order = 20)]
    public string PageAnchor { get; set; } = string.Empty;

    [VisibleIfEqualTo(nameof(Mode), ServiceWidgetModel.SELECT_PAGE, StringComparison.OrdinalIgnoreCase)]
    [ContentItemSelectorComponent(
        ServicePage.CONTENT_TYPE_NAME,
        Label = "Select service page",
        ExplanationText = "Choose the service page to be displayed in the widget.",
        MaximumItems = 1,
        Order = 30)]
    public IEnumerable<ContentItemReference> SelectedServicePage { get; set; } = [];

    [CheckBoxComponent(
        Label = "Display service image",
        Order = 40)]
    public bool ShowServiceImage { get; set; } = true;

    [CheckBoxComponent(
        Label = "Display service benefits",
        Order = 50)]
    public bool ShowServiceBenefits { get; set; } = true;

    [CheckBoxComponent(
        Label = "Display service features",
        Order = 60)]
    public bool ShowServiceFeatures { get; set; } = false;

    [TextInputComponent(
        Label = "Call to action (CTA) text",
        ExplanationText = "Add a call to action text, e.g., \"Read more\".",
        Order = 70)]
    public string CallToAction { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "Open in new tab",
        ExplanationText = "Opens Service page in new tab when visitor clicks the widget or CTA",
        Order = 80)]
    public bool OpenInNewTab { get; set; } = true;

    // advanced styling configuration
    [CheckBoxComponent(
        Label = "Show advanced options",
        Order = 90)]
    public bool ShowAdvanced { get; set; } = false;

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "Color scheme",
        ExplanationText = "Select widget color scheme.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ColorSchemeOption>),
        Order = 100)]
    public string ColorScheme { get; set; } = nameof(ColorSchemeOption.Dark1);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "Corner style",
        DataProviderType = typeof(DropdownEnumOptionProvider<CornerStyleOption>),
        Order = 110)]
    public string CornerStyle { get; set; } = nameof(CornerStyleOption.Sharp);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [CheckBoxComponent(
        Label = "Drop shadow",
        Order = 120)]
    public bool DropShadow { get; set; } = false;

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [VisibleIfTrue(nameof(ShowServiceImage))]
    [DropDownComponent(
        Label = "Image position",
        ExplanationText = "Select the image position with respect to text.",
        DataProviderType = typeof(DropdownEnumOptionProvider<ImagePositionOption>),
        Order = 130)]
    public string ImagePosition { get; set; } = nameof(ImagePositionOption.FullWidth);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [DropDownComponent(
        Label = "Text alignment",
        DataProviderType = typeof(DropdownEnumOptionProvider<ContentAlignmentOption>),
        Order = 140)]
    public string TextAlignment { get; set; } = nameof(ContentAlignmentOption.Left);

    [VisibleIfTrue(nameof(ShowAdvanced))]
    [VisibleIfNotEmpty(nameof(CallToAction))]
    [DropDownComponent(
        Label = "CTA button style",
        DataProviderType = typeof(DropdownEnumOptionProvider<LinkStyleOption>),
        Order = 150)]
    public string CallToActionStyle { get; set; } = nameof(LinkStyleOption.Medium);
}

public enum ImagePositionOption
{
    [Description("Full width")]
    FullWidth,
    [Description("Left side")]
    Left,
    [Description("Right side")]
    Right,
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

public static class ServiceWidgetModel
{
    public const string CURRENT_PAGE = "currentServicePage";
    public const string CURRENT_PAGE_DESCRIPTION = "Use the current service page. (If this widget is placed on existing Service page, automatically displays the correspongind Service data.)";

    public const string SELECT_PAGE = "servicePage";
    public const string SELECT_PAGE_DESCRIPTION = "Select a service page.";
}