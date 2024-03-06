using System.Globalization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;

using TrainingGuides.Web.Features.Products.Models;

namespace TrainingGuides.Web.Features.Products.TagHelpers;

/// <summary>
/// Formats Product feature value based on its type.
/// </summary>
[HtmlTargetElement("tg-product-feature-value", TagStructure = TagStructure.WithoutEndTag)]
public class ProductFeatureValueTagHelper : TagHelper
{
    private readonly IHttpContextAccessor accessor;
    public ProductFeatureViewModel? Feature { get; set; }

    private const string OPENING_TAG = "<span>";
    private const string CLOSING_TAG = "</span>";

    public ProductFeatureValueTagHelper(IHttpContextAccessor accessor)
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

        string? formattedValue = Feature?.ValueType switch
        {
            ProductFeatureValueType.Text => Feature.Value.Value,
            ProductFeatureValueType.Number => string.Format(CultureInfo.CurrentUICulture, "{0:0.00}", Feature.Price),
            ProductFeatureValueType.Boolean => Feature.FeatureIncluded ? "✔" : "-",
            _ => string.Empty
        };

        output.Content.SetHtmlContent(new HtmlString(OPENING_TAG + formattedValue + CLOSING_TAG));
    }
}