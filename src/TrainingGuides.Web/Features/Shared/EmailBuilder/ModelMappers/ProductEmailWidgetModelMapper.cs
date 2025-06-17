using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.ModelMappers;

public class ProductEmailWidgetModelMapper : IComponentModelMapper<ProductWidgetModel>
{

    private readonly IContentItemRetrieverService<ProductPage> productPageRetrieverService;

    public ProductEmailWidgetModelMapper(
        IContentItemRetrieverService<ProductPage> productPageRetrieverService)
    {
        this.productPageRetrieverService = productPageRetrieverService;
    }

    public async Task<ProductWidgetModel> Map(Guid webPageItemContentItemGuid, string languageName)
    {
        var page = await productPageRetrieverService.RetrieveWebPageByContentItemGuid(
            contentItemGuid: webPageItemContentItemGuid,
            contentTypeName: ProductPage.CONTENT_TYPE_NAME,
            depth: 2,
            languageName: languageName);

        var product = page?.ProductPageProduct.FirstOrDefault();

        // If the product or page is null, return an empty model. Note the product will always be null if the page is null.
        if (product is null)
        {
            return new ProductWidgetModel();
        }

        string webPageItemUrl = page.GetUrl().AbsoluteUrl;

        var image = product.ProductMedia.FirstOrDefault();
        string imageUrl = image?.AssetFile?.Url ?? string.Empty;

        return new ProductWidgetModel
        {
            Name = product.ProductName,
            Description = product.ProductDescription,
            Url = webPageItemUrl,
            ImageUrl = imageUrl,
            ImageAltText = image?.AssetAltText ?? image?.AssetDescription ?? string.Empty,
        };
    }
}