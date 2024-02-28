using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.Products.Models;

public class ProductFeaturesViewModel
{
    public string Key { get; set; } = null!;
    public string Name { get; set; } = null!;
    public HtmlString Label { get; set; } = null!;
    public decimal Price { get; set; }
    public HtmlString Value { get; set; } = null!;
    public bool FeatureIncluded { get; set; }
    public ProductFeatureValueType ValueType { get; set; }
    public bool ShowInComparator { get; set; }

    public static ProductFeaturesViewModel GetViewModel(ProductFeature feature) => new()
    {
        Key = feature.ProductFeatureKey,
        Name = feature.SystemFields.ContentItemName,
        Label = new(feature.ProductFeatureLabel),
        Price = feature.ProductFeaturePrice,
        Value = new(feature.ProductFeatureValue),
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
