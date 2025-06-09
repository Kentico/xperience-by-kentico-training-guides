using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.ModelMappers;

public class ProductEmailWidgetModelMapper : IComponentModelMapper<ProductWidgetModel>
{

    private readonly IContentItemRetrieverService<ProductPage> productPageRetrieverService;
    private readonly IHttpRequestService httpRequestService;

    public ProductEmailWidgetModelMapper(
        IContentItemRetrieverService<ProductPage> productPageRetrieverService,
        IHttpRequestService httpRequestService)
    {
        this.productPageRetrieverService = productPageRetrieverService;
        this.httpRequestService = httpRequestService;
    }

    public async Task<ProductWidgetModel> Map(Guid webPageItemContentItemGuid, string languageName)
    {
        var page = await productPageRetrieverService.RetrieveWebPageByContentItemGuid(
            contentItemGuid: webPageItemContentItemGuid,
            contentTypeName: ProductPage.CONTENT_TYPE_NAME,
            depth: 2,
            languageName: languageName);

        var product = page?.ProductPageProduct.FirstOrDefault();

        if (product is null || page is null)
        {
            return new ProductWidgetModel();
        }

        string webPageItemUrl = httpRequestService.GetAbsoluteUrlForPath(page.SystemFields.WebPageUrlPath, false);

        var image = product.ProductMedia.FirstOrDefault();
        string imageUrl = httpRequestService.GetAbsoluteUrlForPath(image?.AssetFile?.Url ?? string.Empty, false);

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