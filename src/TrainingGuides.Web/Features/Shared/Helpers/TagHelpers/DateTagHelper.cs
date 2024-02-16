using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;

namespace TrainingGuides.Web.Features.Shared.Helpers.TagHelpers;

/// <summary>
/// Formatts DateTime value into 'd MMMM yyyy' shape.
/// </summary>
[HtmlTargetElement("tg-date", TagStructure = TagStructure.WithoutEndTag)]
public class DateTagHelper : TagHelper
{
    private readonly IHttpContextAccessor accessor;
    public DateTime? Date { get; set; }

    private const string OPENING_TAG = "<div class=\"c-date\">";
    private const string CLOSING_TAG = "</div>";

    public DateTagHelper(IHttpContextAccessor accessor)
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
        var formattedDate = Date?.Year > 1900 ? new HtmlString(Date?.ToString("d MMMM yyyy")) : null;

        output.Content.SetHtmlContent(new HtmlString(OPENING_TAG + formattedDate + CLOSING_TAG));
    }
}