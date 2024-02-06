using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TrainingGuides.Web.Features.Shared.OptionsProviders.Heading;

[HtmlTargetElement("xpc-heading", TagStructure = TagStructure.NormalOrSelfClosing)]
public class HeadingTagHelper : TagHelper
{
    private readonly IHttpContextAccessor accessor;

    public HeadingTagHelper(IHttpContextAccessor accessor)
    {
        this.accessor = accessor;
    }

    [HtmlAttributeName("headingType")]
    public HeadingTypeOption HeadingType { get; set; }

    [HtmlAttributeName("headingTypeDefault")]
    public HeadingTypeOption HeadingTypeDefault { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var httpContext = accessor.HttpContext;

        if (HeadingType == HeadingTypeOption.Auto)
            HeadingType = HeadingTypeDefault;

        output.TagName = HeadingType.ToString();
    }
}
