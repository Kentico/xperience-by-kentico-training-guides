using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TrainingGuides.Web.Features.Shared.OptionsProviders.Heading;

[HtmlTargetElement("tg-heading", TagStructure = TagStructure.NormalOrSelfClosing)]
public class HeadingTagHelper : TagHelper
{
    private readonly IHttpContextAccessor accessor;

    public HeadingTagHelper(IHttpContextAccessor accessor)
    {
        this.accessor = accessor;
    }

    [HtmlAttributeName("headingType")]
    public HeadingTypeOptions HeadingType { get; set; }

    [HtmlAttributeName("headingTypeDefault")]
    public HeadingTypeOptions HeadingTypeDefault { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var httpContext = accessor.HttpContext;

        if (HeadingType == HeadingTypeOptions.Auto)
            HeadingType = HeadingTypeDefault;

        output.TagName = HeadingType.ToString();
    }
}
