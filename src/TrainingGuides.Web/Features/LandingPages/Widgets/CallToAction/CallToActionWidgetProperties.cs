using CMS.ContentEngine;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using Kentico.Xperience.Admin.Websites.FormAnnotations;

namespace TrainingGuides.Web.Features.LandingPages.Widgets.CallToAction;

public class CallToActionWidgetProperties : IWidgetProperties
{
    [TextInputComponent(
        Label = "{$TrainingGuides.CallToActionWidget.Text.Label$}",
        ExplanationText = "{$TrainingGuides.CallToActionWidget.Text.ExplanationText$}",
        Order = 10)]
    public string Text { get; set; } = string.Empty;

    [RadioGroupComponent(
        Label = "{$TrainingGuides.CallToActionWidget.Type.Label$}",
        ExplanationText = "{$TrainingGuides.CallToActionWidget.Type.ExplanationText$}",
        Options = "page;{$TrainingGuides.CallToActionWidget.Type.Options.Page$}\ncontent;{$TrainingGuides.CallToActionWidget.Type.Options.Content$}\nabsolute;{$TrainingGuides.CallToActionWidget.Type.Options.Absolute$}",
        Order = 20)]
    public string Type { get; set; } = "page";

    [UrlSelectorComponent(
        Label = "{$TrainingGuides.CallToActionWidget.TargetPage.Label$}",
        ExplanationText = "{$TrainingGuides.CallToActionWidget.TargetPage.ExplanationText$}",
        Order = 30)]
    [VisibleIfEqualTo(nameof(Type), "page", StringComparison.OrdinalIgnoreCase)]
    public string TargetPage { get; set; } = string.Empty;

    [ContentItemSelectorComponent(
        Asset.CONTENT_TYPE_NAME,
        Label = "{$TrainingGuides.CallToActionWidget.ContentItem.Label$}",
        ExplanationText = "{$TrainingGuides.CallToActionWidget.ContentItem.ExplanationText$}",
        Order = 40)]
    [VisibleIfEqualTo(nameof(Type), "content", StringComparison.OrdinalIgnoreCase)]
    public IEnumerable<ContentItemReference> ContentItem { get; set; } = [];

    [TextInputComponent(
        Label = "{$TrainingGuides.CallToActionWidget.AbsoluteUrl.Label$}",
        ExplanationText = "{$TrainingGuides.CallToActionWidget.AbsoluteUrl.ExplanationText$}",
        Order = 50)]
    [VisibleIfEqualTo(nameof(Type), "absolute", StringComparison.OrdinalIgnoreCase)]
    public string AbsoluteUrl { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "{$TrainingGuides.CallToActionWidget.IsDownload.Label$}",
        ExplanationText = "{$TrainingGuides.CallToActionWidget.IsDownload.ExplanationText$}",
        Order = 60)]
    [VisibleIfEqualTo(nameof(Type), "absolute", StringComparison.OrdinalIgnoreCase)]
    public bool IsDownload { get; set; } = false;

    [TextInputComponent(
        Label = "{$TrainingGuides.CallToActionWidget.Identifier.Label$}",
        ExplanationText = "{$TrainingGuides.CallToActionWidget.Identifier.ExplanationText$}",
        Order = 70)]
    public string Identifier { get; set; } = string.Empty;

    [CheckBoxComponent(
        Label = "{$TrainingGuides.CallToActionWidget.OpenInNewTab.Label$}",
        Order = 80)]
    public bool OpenInNewTab { get; set; }
}