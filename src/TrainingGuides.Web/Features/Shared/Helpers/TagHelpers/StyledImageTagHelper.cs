using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Shared.Helpers.TagHelpers;

[HtmlTargetElement("tg-styled-image", TagStructure = TagStructure.WithoutEndTag)]
public class StyledImageTagHelper(
    IComponentStyleEnumService componentStyleEnumService) : TagHelper
{
    public string CornerStyle { get; set; } = string.Empty;

    private const string IMG_TAG = "img";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = IMG_TAG;
        output.TagMode = TagMode.SelfClosing;

        List<string> cssClasses = [];

        var cornerStyle = componentStyleEnumService.GetCornerStyle(CornerStyle ?? string.Empty);
        cssClasses.AddRange(componentStyleEnumService.GetCornerStyleClasses(cornerStyle));

        if (cssClasses.Count > 0)
        {
            foreach (string cssClass in cssClasses)
            {
                output.AddClass(cssClass, HtmlEncoder.Default);
            }
        }
    }
}
