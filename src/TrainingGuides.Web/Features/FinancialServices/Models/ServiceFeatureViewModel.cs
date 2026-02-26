using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.FinancialServices.Models;

public class ServiceFeatureViewModel
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public HtmlString LabelHtml { get; set; } = HtmlString.Empty;
    public decimal Price { get; set; }
    public HtmlString ValueHtml { get; set; } = HtmlString.Empty;
    public bool FeatureIncluded { get; set; }
    public ServiceFeatureValueType ValueType { get; set; }
    public bool ShowInComparator { get; set; }

    public static ServiceFeatureViewModel GetViewModel(ServiceFeature feature) => new()
    {
        Key = feature.ServiceFeatureKey,
        Name = feature.SystemFields.ContentItemName,
        LabelHtml = new(feature.ServiceFeatureLabel),
        Price = feature.ServiceFeaturePrice,
        ValueHtml = new(feature.ServiceFeatureValue),
        FeatureIncluded = feature.ServiceFeatureIncluded,
        ValueType = GetValueType(feature.ServiceFeatureValueType),
        ShowInComparator = feature.ServiceFeatureShowInComparator == "1"
    };

    private static ServiceFeatureValueType GetValueType(string value)
    {
        if (string.IsNullOrEmpty(value))
            return ServiceFeatureValueType.Text;

        if (int.TryParse(value, out int id))
            return (ServiceFeatureValueType)id;

        return ServiceFeatureValueType.Text;
    }
}

public enum ServiceFeatureValueType
{
    Text = 0,
    Number = 1,
    Boolean = 2
}
