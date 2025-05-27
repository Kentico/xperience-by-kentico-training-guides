using Kentico.Xperience.Mjml.StarterKit.Rcl.Mapping;
using Kentico.Xperience.Mjml.StarterKit.Rcl.Widgets;
using TrainingGuides.Web.Features.Shared.Services;

namespace TrainingGuides.Web.Features.Shared.EmailBuilder.ModelMappers;

public class ProductEmailWidgetModelMapper
    : IComponentModelMapper<ProductWidgetModel>
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

    public async Task<ProductWidgetModel> Map(Guid webPageItemGuid, string languageName)
    {
        var page = await productPageRetrieverService.RetrieveWebPageByContentItemGuid(webPageItemGuid, ProductPage.CONTENT_TYPE_NAME, 2, languageName);
        var product = page?.ProductPageProduct.FirstOrDefault();

        if (page is null || product is null)
        {
            return new ProductWidgetModel();
        }

        string webPageItemUrl = httpRequestService.GetAbsoluteUrlForPath(page.SystemFields.WebPageUrlPath, false);

        var image = product.ProductMedia.FirstOrDefault();

        return new ProductWidgetModel
        {
            Name = product.ProductName,
            Description = product.ProductDescription,
            Url = webPageItemUrl,
            ImageUrl = image?.AssetFile?.Url ?? string.Empty,
            ImageAltText = image?.AssetDescription ?? string.Empty,
        };
    }
}