using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;
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

    private const string INSTRUCTIONS_EDIT_MODE = "<p class=\"m-5\">This widget needs some setup. Click the <strong>Configure widget</strong> icon in the top right to configure content and design for this widget.</p>";
    private const string INSTRUCTIONS_PREVIEW_MODE = "<p class=\"m-5\">This widget needs some setup. Switch to the <strong>Page Builder</strong> and <strong>Edit page</strong>.<br/>Then click the <strong>Configure widget</strong> icon in the top right to configure content and design for this widget.";

    public ConfigureWidgetInstructionsTagHelper(IHttpContextAccessor accessor)
    {
        this.accessor = accessor;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var httpContext = accessor.HttpContext;

        if (!httpContext.Kentico().PageBuilder().EditMode && !httpContext.Kentico().Preview().Enabled)
        {
            output.SuppressOutput();
        }

        output.TagName = "";
        output.Content.SetHtmlContent(
            httpContext.Kentico().PageBuilder().EditMode
                ? INSTRUCTIONS_EDIT_MODE
                : INSTRUCTIONS_PREVIEW_MODE
            );
    }
}