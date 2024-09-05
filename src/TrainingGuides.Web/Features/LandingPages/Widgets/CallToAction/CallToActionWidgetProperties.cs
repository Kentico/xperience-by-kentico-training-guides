using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;

namespace TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;

public class CallToActionWidgetProperties : IWidgetProperties
{
    [TextInputComponent(
        Label = "CTA button text",
        ExplanationText = "Add your call to action. Keep it under 30 characters.",
        Order = 10)]
    public string Text { get; set; } = string.Empty;

    [RadioGroupComponent(
        Label = "Target content",
        ExplanationText = "Select what happens when a visitor clicks your CTA.",
        Options = "page;Page\ncontent;Content hub asset\nabsolute;Absolute URL",
        Order = 20)]
    public string Type { get; set; } = "page";

    [UrlSelectorComponent(
        Label = "Target page",
        ExplanationText = "Select the page in the tree.",
        Order = 30)]
    [VisibleIfEqualTo(nameof(Type), "page", StringComparison.OrdinalIgnoreCase)]
    public string TargetPage { get; set; } = string.Empty;

    [ContentItemSelectorComponent(
        Asset.CONTENT_TYPE_NAME,
        Label = "Content hub asset",
        ExplanationText = "Select a file (image, video, PDF, etc. that opens when a visitor clicks your CTA.\r\n",
        Order = 40)]
    [VisibleIfEqualTo(nameof(Type), "content", StringComparison.OrdinalIgnoreCase)]
    public IEnumerable<ContentItemReference> ContentItem { get; set; } = Enumerable.Empty<ContentItemReference>();

    [TextInputComponent(
        Label = "Absolute URL",
        ExplanationText = "Add a hyperlink to an external site, or use the product's URL + anchor tag # for referencing an anchor on the page, for example, \"https://your-doma.in/contact-us#form\"",
        Order = 50)]
    [VisibleIfEqualTo(nameof(Type), "absolute", StringComparison.OrdinalIgnoreCase)]
    public string AbsoluteUrl { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "File download",
        ExplanationText = "Select if you want to log “File download activity” to the contact.",
        Order = 60)]
    [VisibleIfEqualTo(nameof(Type), "absolute", StringComparison.OrdinalIgnoreCase)]
    public bool IsDownload { get; set; } = false;

    [TextInputComponent(
        Label = "CTA unique data identifier",
        ExplanationText = "Use this field to set an identifier of this widget instance. For example, it can come in handy for logging custom activities or distinguishing personalization variants.",
        Order = 70)]
    public string Identifier { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "Open in new tab",
        Order = 80)]
    public bool OpenInNewTab { get; set; }
}