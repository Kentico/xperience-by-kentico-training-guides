using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Text.Encodings.Web;

namespace TrainingGuides.Web.Features.Shared.Helpers.TagHelpers;

/// <summary>
/// Formats DateTime value into 'd MMMM yyyy' shape.
/// </summary>
[HtmlTargetElement("tg-date", TagStructure = TagStructure.WithoutEndTag)]
public class DateTagHelper : TagHelper
{
    public DateTime? Date { get; set; }

    private const string DIV_TAG = "div";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = DIV_TAG;
        output.TagMode = TagMode.StartTagAndEndTag;
        var formattedDate = Date?.Year > 1900 ? new HtmlString(Date?.ToString("d MMMM yyyy")) : null;
        output.AddClass("c-date", HtmlEncoder.Default);
        output.Content.SetHtmlContent(formattedDate);
    }
}