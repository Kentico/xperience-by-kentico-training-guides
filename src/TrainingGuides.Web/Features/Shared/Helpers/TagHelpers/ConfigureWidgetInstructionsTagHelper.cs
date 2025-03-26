using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TrainingGuides.Web.Features.Shared.Helpers.TagHelpers;

/// <summary>
/// Displays a helpful message that a Widget needs to be configured when the page is viewed
/// in Page Builder or Preview mode.
/// </summary>
[HtmlTargetElement("tg-configure-widget-instructions", TagStructure = TagStructure.WithoutEndTag)]
public class ConfigureWidgetInstructionsTagHelper : TagHelper
{
    private readonly IHttpContextAccessor accessor;
    public string Message { get; set; } = string.Empty;

    private const string P_TAG = "p";
    private const string INSTRUCTIONS_EDIT_MODE = "This widget needs some setup. Click the <strong>Configure widget</strong> gear icon in the top right to configure content and design for this widget.";
    private const string INSTRUCTIONS_READONLY_MODE = "This widget needs some setup. Click <strong>Edit page</strong> and then the <strong>Configure widget</strong> gear icon in the top right to configure content and design for this widget.";
    private const string INSTRUCTIONS_PREVIEW_MODE = "This widget needs some setup. Switch to the <strong>Page Builder</strong> and <strong>Edit page</strong>.<br/>Then click the <strong>Configure widget</strong> gear icon in the top right to configure content and design for this widget.";

    public ConfigureWidgetInstructionsTagHelper(IHttpContextAccessor accessor)
    {
        this.accessor = accessor;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = P_TAG;
        output.TagMode = TagMode.StartTagAndEndTag;

        var httpContext = accessor.HttpContext;
        string messageToShow = string.IsNullOrEmpty(Message)
            ? (httpContext.Kentico().PageBuilder().GetMode() switch
            {
                PageBuilderMode.Edit => INSTRUCTIONS_EDIT_MODE,
                PageBuilderMode.ReadOnly => INSTRUCTIONS_READONLY_MODE,
                PageBuilderMode.Off => INSTRUCTIONS_PREVIEW_MODE,
                _ => INSTRUCTIONS_PREVIEW_MODE
            })
            : Message;

        output.Content.SetHtmlContent(new HtmlString(messageToShow));
    }
}