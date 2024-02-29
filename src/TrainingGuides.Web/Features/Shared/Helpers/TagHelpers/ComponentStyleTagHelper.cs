using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TrainingGuides.Web.Features.Shared.Services;


namespace TrainingGuides.Web.Features.Shared.Helpers.TagHelpers;

[HtmlTargetElement("tg-component-style")]
public class ComponentStyleTagHelper : TagHelper
{
    public string? ColorScheme { get; set; }
    public string? CornerStyle { get; set; }

    private readonly IComponentStyleEnumService componentStyleEnumService;

    public ComponentStyleTagHelper(IComponentStyleEnumService componentStyleEnumService) : base()
    {
        this.componentStyleEnumService = componentStyleEnumService;
    }
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";

        List<string> cssClasses = [];

        var colorScheme = componentStyleEnumService.GetColorScheme(ColorScheme ?? string.Empty);
        cssClasses.AddRange(componentStyleEnumService.GetColorSchemeClasses(colorScheme));

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
