using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

using TrainingGuides.Web.Features.FinancialServices.Models;

namespace TrainingGuides.Web.Features.FinancialServices.TagHelpers;

/// <summary>
/// Formats Service feature value based on its type.
/// </summary>
[HtmlTargetElement("tg-service-feature-value", TagStructure = TagStructure.WithoutEndTag)]
public class ServiceFeatureValueTagHelper : TagHelper
{
    public ServiceFeatureViewModel? Feature { get; set; }

    private const string SPAN_TAG = "span";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = SPAN_TAG;
        output.TagMode = TagMode.StartTagAndEndTag;

        string? formattedValue = Feature?.ValueType switch
        {
            ServiceFeatureValueType.Text => Feature.ValueHtml.Value,
            ServiceFeatureValueType.Number => "$" + Feature.Price.ToString("n2"),
            ServiceFeatureValueType.Boolean => Feature.FeatureIncluded ? "✔" : "-",
            _ => string.Empty
        };

        output.Content.SetHtmlContent(new HtmlString(formattedValue));
    }
}