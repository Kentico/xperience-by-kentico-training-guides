using Microsoft.AspNetCore.Razor.TagHelpers;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;

namespace TrainingGuides.Web.Features.Shared.Helpers.TagHelpers;

/// <summary>
/// Displays the inner content of the Tag Helper only when the page is being viewed
/// in Page Builder or Preview mode
/// </summary>
[HtmlTargetElement("tg-page-builder-content", TagStructure = TagStructure.NormalOrSelfClosing)]
public class PageBuilderContentTagHelper : TagHelper
{
    private readonly IHttpContextAccessor accessor;

    public PageBuilderContentTagHelper(IHttpContextAccessor accessor)
    {
        this.accessor = accessor;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var httpContext = accessor.HttpContext;

        bool isInEditMode = httpContext.Kentico().PageBuilder().EditMode;
        bool isInPreview = httpContext.Kentico().Preview().Enabled;

        if (!isInEditMode && !isInPreview)
        {
            output.SuppressOutput();
        }

        output.TagName = "";
    }
}
