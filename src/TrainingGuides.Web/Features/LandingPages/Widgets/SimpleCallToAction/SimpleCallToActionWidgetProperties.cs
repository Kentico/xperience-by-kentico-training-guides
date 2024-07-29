using System.ComponentModel;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;
using TrainingGuides.Web.Features.Shared.OptionProviders;

namespace TrainingGuides.Web.Features.LandingPages.Widgets.SimpleCallToAction;

public class SimpleCallToActionWidgetProperties : IWidgetProperties
{
    [TextInputComponent(
        Label = "Call to action text",
        ExplanationText = "Add your call to action. Keep it under 30 characters.",
        Order = 10)]
    public string? Text { get; set; }

    // See the same property implemented using RadioGroupComponent instead in CallToAction widget.
    [DropDownComponent(
        Label = "Target content",
        ExplanationText = "Select what happens when a visitor clicks your button.",
        DataProviderType = typeof(DropdownEnumOptionProvider<TargetContentOption>),
        Order = 20)]
    public string TargetContent { get; set; } = nameof(TargetContentOption.Page);

    [WebPageSelectorComponent(
        Label = "Target page",
        ExplanationText = "Select the page in the tree.",
        MaximumPages = 1,
        Order = 30)]
    [VisibleIfEqualTo(nameof(TargetContent), nameof(TargetContentOption.Page), StringComparison.OrdinalIgnoreCase)]
    public IEnumerable<WebPageRelatedItem>? TargetContentPage { get; set; }

    [TextInputComponent(
        Label = "Absolute URL",
        ExplanationText = "Add a hyperlink to an external site, or use the product's URL + anchor tag # for referencing an anchor on the page, for example, \"https://your-doma.in/contact-us#form\"",
        Order = 40)]
    [VisibleIfEqualTo(nameof(TargetContent), nameof(TargetContentOption.AbsoluteUrl), StringComparison.OrdinalIgnoreCase)]
    public string? TargetContentAbsoluteUrl { get; set; }

    [CheckBoxComponent(
        Label = "Open in new tab",
        Order = 50)]
    public bool OpenInNewTab { get; set; } = false;
}

public enum TargetContentOption
{
    [Description("Page")]
    Page,
    [Description("Absolute URL")]
    AbsoluteUrl
}