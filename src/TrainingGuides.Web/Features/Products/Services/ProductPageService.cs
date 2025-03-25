using Microsoft.IdentityModel.Tokens;
using TrainingGuides.Web.Features.Products.Models;
using TrainingGuides.Web.Features.Shared.Models;

namespace TrainingGuides.Web.Features.Products.Services;

public class ProductPageService : IProductPageService
{
    private readonly IWebPageUrlRetriever webPageUrlRetriever;
    public ProductPageService(IWebPageUrlRetriever webPageUrlRetriever)
    {
        this.webPageUrlRetriever = webPageUrlRetriever;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ProductPageViewModel"/>, setting the properties using ProductPage given as a parameter.
    /// </summary>
    /// <param name="productPage">Corresponding Product page object.</param>
    /// <returns>New instance of ProductPageViewModel.</returns>
    public async Task<ProductPageViewModel> GetProductPageViewModel(
        ProductPage? productPage,
        bool getMedia = true,
        bool getFeatures = true,
        bool getBenefits = true,
        string callToActionText = "",
        string callToActionLink = "",
        bool openInNewTab = true,
        bool getPrice = true)
    {
        if (productPage == null)
        {
            return new ProductPageViewModel();
        }

        string url = callToActionLink.IsNullOrEmpty()
            ? (await webPageUrlRetriever.Retrieve(productPage)).RelativePath
            : callToActionLink;
        return new ProductPageViewModel
        {
            Name = new(productPage.ProductPageProduct.FirstOrDefault()?.ProductName),
            ShortDescription = new(productPage.ProductPageProduct.FirstOrDefault()?.ProductShortDescription),
            Description = new(productPage.ProductPageProduct.FirstOrDefault()?.ProductDescription),
            Media = getMedia
                ? productPage.ProductPageProduct.FirstOrDefault()?.ProductMedia.Select(AssetViewModel.GetViewModel)?.ToList() ?? []
                : [],
            Link = new LinkViewModel()
            {
                Name = productPage.ProductPageProduct.FirstOrDefault()?.ProductName ?? string.Empty,
                LinkUrl = url,
                CallToAction = callToActionText.IsNullOrEmpty() ? string.Empty : callToActionText,
                OpenInNewTab = openInNewTab
            },
            Features = getFeatures
                ? productPage.ProductPageProduct.FirstOrDefault()?.ProductFeatures
                    .Select(ProductFeatureViewModel.GetViewModel)
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
