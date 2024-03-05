using TrainingGuides.Web.Features.Shared.Models;
using Microsoft.AspNetCore.Html;

namespace TrainingGuides.Web.Features.Products.Models;

public class ProductPageViewModel : PageViewModel
{
    public HtmlString? Name { get; set; }
    public string? Header { get; set; }
    public HtmlString? ShortDescription { get; set; }
    public string? Description { get; set; }
    public List<AssetViewModel> Media { get; set; } = [];
    public LinkViewModel? Link { get; set; }
    public string? CallToAction { get; set; } = null!;
    public decimal Price { get; set; }
    public List<ProductFeaturesViewModel> Features { get; set; } = [];
    public List<BenefitViewModel> Benefits { get; set; } = [];

    public static ProductPageViewModel GetViewModel(
        ProductPage productPage,
        bool getMedia = true,
        bool getFeatures = true,
        bool getBenefits = true,
        bool getCallToAction = true,
        bool getPrice = true)
    {
        if (productPage == null)
        {
            return new ProductPageViewModel();
        }
        return new ProductPageViewModel
        {
            Name = new(productPage.ProductPageProduct.FirstOrDefault()?.ProductName),
            Header = productPage.ProductPageProduct.FirstOrDefault()?.ProductName ?? string.Empty,
            ShortDescription = new(productPage.ProductPageProduct.FirstOrDefault()?.ProductShortDescription),
            Description = productPage.ProductPageProduct.FirstOrDefault()?.ProductDescription ?? string.Empty,
            Media = getMedia
                ? productPage.ProductPageProduct.FirstOrDefault()?.ProductMedia.Select(AssetViewModel.GetViewModel)?.ToList() ?? []
                : [],
            Link = new LinkViewModel()
            {
                Page = productPage.SystemFields.WebPageUrlPath
            },
            CallToAction = getCallToAction ? "See more" : null,
            Features = getFeatures
                ? productPage.ProductPageProduct.FirstOrDefault()?.ProductFeatures
                    .Select(ProductFeaturesViewModel.GetViewModel)
                    .ToList() ?? []
                : [],
            Benefits = getBenefits
                ? productPage.ProductPageProduct.FirstOrDefault()?.ProductBenefits
                    .Select(BenefitViewModel.GetViewModel)
                    .ToList() ?? []
                : [],
            Price = getPrice ? productPage.ProductPageProduct.FirstOrDefault()?.ProductPrice ?? 0 : 0,
        };
    }
}
