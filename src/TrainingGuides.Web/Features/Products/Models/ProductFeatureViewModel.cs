using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.Products.Models;

public class ProductFeatureViewModel
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public HtmlString LabelHtml { get; set; } = HtmlString.Empty;
    public decimal Price { get; set; }
    public HtmlString ValueHtml { get; set; } = HtmlString.Empty;
    public bool FeatureIncluded { get; set; }
    public ProductFeatureValueType ValueType { get; set; }
    public bool ShowInComparator { get; set; }

    public static ProductFeatureViewModel GetViewModel(ProductFeature feature) => new()
    {
        Key = feature.ProductFeatureKey,
        Name = feature.SystemFields.ContentItemName,
        LabelHtml = new(feature.ProductFeatureLabel),
        Price = feature.ProductFeaturePrice,
        ValueHtml = new(feature.ProductFeatureValue),
        FeatureIncluded = feature.ProductFeatureIncluded,
        ValueType = GetValueType(feature.ProductFeatureValueType),
        ShowInComparator = feature.ProductFeatureShowInComparator == "1"
    };

    private static ProductFeatureValueType GetValueType(string value)
    {
        if (string.IsNullOrEmpty(value))
            return ProductFeatureValueType.Text;

        if (int.TryParse(value, out int id))
            return (ProductFeatureValueType)id;

        return ProductFeatureValueType.Text;
    }
}

public enum ProductFeatureValueType
{
    Text = 0,
    Number = 1,
    Boolean = 2
}
