using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;

namespace TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;

public class CallToActionWidgetProperties : IWidgetProperties
{
    [TextInputComponent(Order = 0, Label = "CTA button text", ExplanationText = "Add your call to action. Keep it under 30 characters.")]
    public string? Text { get; set; }

    [RadioGroupComponent(Label = "Target content", ExplanationText = "Select what happens when a visitor clicks your CTA.", Options = "page;Page\ncontent;Content hub asset\nabsolute;Absolute URL", Order = 1)]
    public string Type { get; set; } = "page";

    [UrlSelectorComponent(Label = "Target page", Order = 2, ExplanationText = "Select the page in the tree.")]
    [VisibleIfEqualTo(nameof(Type), "page", StringComparison.OrdinalIgnoreCase)]
    public string? TargetPage { get; set; }

    [ContentItemSelectorComponent(Asset.CONTENT_TYPE_NAME, Label = "Content hub asset", Order = 2, ExplanationText = "Select a file (image, video, PDF, etc. that opens when a visitor clicks your CTA.\r\n")]
    [VisibleIfEqualTo(nameof(Type), "content", StringComparison.OrdinalIgnoreCase)]
    public IEnumerable<ContentItemReference>? ContentItem { get; set; }

    [TextInputComponent(Order = 3, Label = "Absolute URL", ExplanationText = "Add a hyperlink to an external site, or use the product's URL + anchor tag # for referencing an anchor on the page, for example, \"https://your-doma.in/contact-us#form\"")]
    [VisibleIfEqualTo(nameof(Type), "absolute", StringComparison.OrdinalIgnoreCase)]
    public string? AbsoluteUrl { get; set; }

    [CheckBoxComponent(Order = 4, Label = "File download", ExplanationText = "Select if you want to log “File download activity” to the contact.")]
    [VisibleIfEqualTo(nameof(Type), "absolute", StringComparison.OrdinalIgnoreCase)]
    public bool IsDownload { get; set; } = false;

    [TextInputComponent(Order = 5, Label = "CTA unique data identifier", ExplanationText = "Use this field to set an identifier of this widget instance. For example, it can come in handy for logging custom activities or distinguishing personalization variants.")]
    public string? Identifier { get; set; }

    [CheckBoxComponent(Order = 6, Label = "Open in new tab")]
    public bool OpenInNewTab { get; set; }
}