namespace TrainingGuides.Web.Commerce.Products.Models;

public class VariantViewModel
{
    public string VariantName { get; set; } = string.Empty;
    public ProductImageViewModel VariantImage { get; set; } = new ProductImageViewModel();
    public string VariantCodeName { get; set; } = string.Empty;
}