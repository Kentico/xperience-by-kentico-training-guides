using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Shared.Helpers.TagHelpers;

public class StyledImageTagHelper : TagHelper
{
    public string CornerType { get; set; }

    private readonly IComponentStyleEnumService componentStyleEnumService;
    public StyledImageTagHelper(IComponentStyleEnumService componentStyleEnumService) : base()
    {
        this.componentStyleEnumService = componentStyleEnumService;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "img";
        output.TagMode = TagMode.SelfClosing;

        List<string> cssClasses = [];

        var cornerType = componentStyleEnumService.GetCornerType(CornerType);
        cssClasses.AddRange(componentStyleEnumService.GetCornerTypeClasses(cornerType));

        if (cssClasses.Count > 0)
        {
            foreach (string cssClass in cssClasses)
            {
                output.AddClass(cssClass, HtmlEncoder.Default);
            }
        }

    }
}
