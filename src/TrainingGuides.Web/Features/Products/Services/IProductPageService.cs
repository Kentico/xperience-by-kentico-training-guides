using TrainingGuides.Web.Features.Products.Models;

namespace TrainingGuides.Web.Features.Products.Services;

public interface IProductPageService
{
    public Task<ProductPageViewModel> GetProductPageViewModel(
        ProductPage productPage,
        bool getMedia = true,
        bool getFeatures = true,
        bool getBenefits = true,
        string callToAction = "",
        bool openInNewTab = true,
        bool getPrice = true);
}