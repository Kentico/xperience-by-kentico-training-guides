using TrainingGuides.Web.Features.Shared.Models;
using Microsoft.AspNetCore.Html;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Products.Models;

public class ProductPageViewModel : PageViewModel
{
    public HtmlString Name { get; set; } = null!;
    public string? Header { get; set; } = null!;
    public HtmlString ShortDescription { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public List<AssetViewModel> Media { get; set; } = [];
    public LinkViewModel? Link { get; set; }
    public string? CallToAction { get; set; } = null!;
    public decimal Price { get; set; }
    public List<ProductFeaturesViewModel> Features { get; set; } = [];

    public static ProductPageViewModel GetViewModel(ProductPage productPage)
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
            Media = productPage.ProductPageProduct.FirstOrDefault()?.ProductMedia.Select(AssetViewModel.GetViewModel)?.ToList(),
            Link = new LinkViewModel()
            {
                Page = productPage.SystemFields.WebPageUrlPath
            },
            CallToAction = "See more",
            Features = productPage.ProductPageProduct.FirstOrDefault()?.ProductFeatures.Select(feature => ProductFeaturesViewModel.GetViewModel(feature)).ToList(),
            Price = productPage.ProductPageProduct.FirstOrDefault()?.ProductPrice ?? 0,
        };
    }
}
