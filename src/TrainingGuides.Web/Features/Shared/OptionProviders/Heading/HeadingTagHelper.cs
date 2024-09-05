using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TrainingGuides.Web.Features.Shared.OptionProviders.Heading;

[HtmlTargetElement("tg-heading", TagStructure = TagStructure.NormalOrSelfClosing)]
public class HeadingTagHelper : TagHelper
{
    [HtmlAttributeName("headingType")]
    public string HeadingType { get; set; } = string.Empty;

    [HtmlAttributeName("headingTypeDefault")]
    public HeadingTypeOption HeadingTypeDefault { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var headingType = new DropdownEnumOptionProvider<HeadingTypeOption>().Parse(string.Empty, HeadingTypeDefault);
        output.TagName = GetSafeTagText(headingType);
    }

    private string GetSafeTagText(HeadingTypeOption messageType) => messageType switch
    {
        HeadingTypeOption.H1 => "h1",
        HeadingTypeOption.H2 => "h2",
        HeadingTypeOption.H3 => "h3",
        HeadingTypeOption.H4 => "h4",
        HeadingTypeOption.H5 => "h5",
        HeadingTypeOption.P => "p",
        _ => "span"
    };
}
