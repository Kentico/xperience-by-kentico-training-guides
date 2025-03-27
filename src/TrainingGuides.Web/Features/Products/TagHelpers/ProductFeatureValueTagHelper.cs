using System.Globalization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

using TrainingGuides.Web.Features.Products.Models;

namespace TrainingGuides.Web.Features.Products.TagHelpers;

/// <summary>
/// Formats Product feature value based on its type.
/// </summary>
[HtmlTargetElement("tg-product-feature-value", TagStructure = TagStructure.WithoutEndTag)]
public class ProductFeatureValueTagHelper : TagHelper
{
    public ProductFeatureViewModel? Feature { get; set; }

    private const string SPAN_TAG = "span";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = SPAN_TAG;
        output.TagMode = TagMode.StartTagAndEndTag;

        string? formattedValue = Feature?.ValueType switch
        {
            ProductFeatureValueType.Text => Feature.ValueHtml.Value,
            ProductFeatureValueType.Number => string.Format(CultureInfo.CurrentUICulture, "{0:0.00}", Feature.Price),
            ProductFeatureValueType.Boolean => Feature.FeatureIncluded ? "✔" : "-",
            _ => string.Empty
        };

        output.Content.SetHtmlContent(new HtmlString(formattedValue));
    }
}